using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using ReactiveUI;
using WaifuGallery.Helpers;
using WaifuGallery.Models;
using WaifuGallery.ViewModels.FileExplorer;
using WaifuGallery.ViewModels.Tabs;

namespace WaifuGallery.ViewModels;

public class MainViewViewModel : ViewModelBase
{
    #region Private Members

    private readonly MainWindow _mainWindow;
    private string _statusBarMessage = "Welcome to WaifuGallery!";
    private bool _isStatusBarVisible = true;

    #endregion

    #region Public Properties

    public MenuBarViewModel MenuBarViewModel { get; init; }
    public TabsViewModel TabsViewModel { get; init; }
    public FileExplorerViewModel FileExplorerViewModel { get; init; }

    public string StatusBarMessage
    {
        get => _statusBarMessage;
        set => this.RaiseAndSetIfChanged(ref _statusBarMessage, $"Information: {value}");
    }

    public bool IsStatusBarVisible
    {
        get => _isStatusBarVisible;
        set => this.RaiseAndSetIfChanged(ref _isStatusBarVisible, value);
    }

    #endregion

    #region CTOR

    public MainViewViewModel(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        MenuBarViewModel = new MenuBarViewModel();
        TabsViewModel = new TabsViewModel();
        FileExplorerViewModel = new FileExplorerViewModel()
        {
            StorageProvider = _mainWindow.StorageProvider
        };
        MenuBarViewModel.OnSendCommandToMainView += HandleControls;
        FileExplorerViewModel.OnSendCommandToMainView += HandleControls;
        TabsViewModel.OnSendCommandToMainView += HandleControls;
    }

    #endregion

    private void HandleControls(object? sender, Command command)
    {
        if (sender is null) return;
        switch (sender.GetType().Name)
        {
            case "MenuBarViewModel":
                HandleMenuBarMessage(command.Type);
                break;
            case "FileExplorerViewModel":
                HandleFileExplorerMessage(command);
                break;
        }
    }

    private void HandleFileExplorerMessage(Command command)
    {
        var imagesInPath = command.ImagesInPath;
        string? path;
        string? parentDirName;
        ImageTabViewModel imageTabViewModel;
        switch (command.Type)
        {
            case CommandType.OpenImageInNewTab:
                if (imagesInPath is null) return;
                path = imagesInPath[command.Index];
                parentDirName = Directory.GetParent(path)?.Name;
                if (parentDirName is null) return;
                imageTabViewModel =
                    new ImageTabViewModel(imagesInPath[command.Index], imagesInPath, command.Index);
                AddTabViewModelToOpenTabs(imageTabViewModel);
                break;
            case CommandType.OpenFolderInNewTab:
                if (imagesInPath is null) return;
                path = imagesInPath.First();
                parentDirName = Directory.GetParent(path)?.Name;
                if (parentDirName is null) return;
                imageTabViewModel = new ImageTabViewModel(parentDirName, imagesInPath, 0);
                AddTabViewModelToOpenTabs(imageTabViewModel);
                break;
            case CommandType.SendMessageToStatusBar:
                if (command.Message != null) StatusBarMessage = command.Message;
                break;
        }
    }

    private void HandleMenuBarMessage(CommandType type)
    {
        switch (type)
        {
            case CommandType.Exit:
                Environment.Exit(0);
                break;
            case CommandType.ToggleFullScreen:
                StatusBarMessage = $"Toggling FullScreen...";
                ToggleFullScreen();
                break;
            case CommandType.OpenFile:
                StatusBarMessage = $"Opening File...";
                break;
            case CommandType.ToggleFileExplorer:
                StatusBarMessage = $"Toggling File Explorer...";
                FileExplorerViewModel.ToggleFileExplorer();
                break;
            case CommandType.ToggleFileExplorerVisibility:
                StatusBarMessage = $"Toggling File Explorer Visibility...";
                FileExplorerViewModel.IsFileExplorerVisible = !FileExplorerViewModel.IsFileExplorerVisible;
                break;
            case CommandType.FitToWidth:
                StatusBarMessage = $"FitToWidth";
                break;
            case CommandType.FitToHeight:
                StatusBarMessage = $"FitToHeight";
                break;
            default:
                StatusBarMessage = "Command not found!";
                break;
        }
    }

    public void HandleKeyboardEvent(KeyEventArgs e)
    {
        if (FileExplorerViewModel.IsSearchFocused) return;
        HandleMainViewKeyboardEvent(e);

        switch (FileExplorerViewModel)
        {
            case {IsFileExplorerVisible: false}:
            case {IsFileExplorerExpanded: false}:
                return;
            default:
                HandleFileExplorerKeyboardEvent(e);
                break;
        }

        e.Handled = true;
    }

    private void HandleFileExplorerKeyboardEvent(KeyEventArgs e)
    {
        if (FileExplorerViewModel.IsSearchFocused) return;
        switch (e)
        {
            case {Key: Key.Back}:
                FileExplorerViewModel.GoToParentFolder();
                break;
            case {Key: Key.Enter}:
                FileExplorerViewModel.ChangePath(FileExplorerViewModel.SelectedFile.FullPath);
                break;
            case {Key: Key.H}:
            case {Key: Key.Left}:
                if (FileExplorerViewModel.PreviewImageViewModel.IsPreviewImageVisible)
                {
                    FileExplorerViewModel.PreviewImageViewModel.PreviousPreview();
                }
                else
                {
                    FileExplorerViewModel.SelectedIndex -= 1;
                }

                break;
            case {Key: Key.J}:
            case {Key: Key.Down}:
                FileExplorerViewModel.GoDown();
                break;
            case {Key: Key.K}:
            case {Key: Key.Up}:
                FileExplorerViewModel.GoUp();
                break;
            case {Key: Key.L}:
            case {Key: Key.Right}:
                if (FileExplorerViewModel.PreviewImageViewModel.IsPreviewImageVisible)
                {
                    FileExplorerViewModel.PreviewImageViewModel.NextPreview();
                }
                else
                {
                    FileExplorerViewModel.SelectedIndex += 1;
                }

                break;
            case {Key: Key.O}:
            case {Key: Key.Space}:
                FileExplorerViewModel.OpenTab();
                break;
            case {Key: Key.P}:
                var imagesInPath =
                    ImagesHelper.GetAllImagesInPathFromString(FileExplorerViewModel.SelectedFile.FullPath);
                FileExplorerViewModel.StartPreview(imagesInPath);
                break;
            case {Key: Key.Escape}:
                FileExplorerViewModel.ClosePreview();
                break;
        }
    }

    private void HandleMainViewKeyboardEvent(KeyEventArgs e)
    {
        switch (e)
        {
            case {Key: Key.F11}:
                ToggleFullScreen();
                break;
            case {Key: Key.F, KeyModifiers: KeyModifiers.None}:
                FileExplorerViewModel.ToggleFileExplorer();
                break;
            case {Key: Key.F, KeyModifiers: KeyModifiers.Shift}:
                FileExplorerViewModel.ToggleFileExplorerVisibility();
                break;
            case {Key: Key.H, KeyModifiers: KeyModifiers.Shift}:
                TabsViewModel.FitToHeight(_mainWindow.Bounds.Size.Height - 50);
                break;
            case {Key: Key.W, KeyModifiers: KeyModifiers.Shift}:
                TabsViewModel.FitToWidth(_mainWindow.Bounds.Size.Width - 50);
                break;
            case {Key: Key.Tab, KeyModifiers: KeyModifiers.Control}:
                switch (FileExplorerViewModel)
                {
                    case {IsFileExplorerVisible: false}:
                    case {IsFileExplorerExpanded: false}:
                        TabsViewModel.SwitchTab();
                        break;
                }

                break;
            case {Key: Key.H}:
            case {Key: Key.Left}:
            case {Key: Key.PageUp}:
                switch (FileExplorerViewModel)
                {
                    case {IsFileExplorerVisible: false}:
                    case {IsFileExplorerExpanded: false}:
                        TabsViewModel.ImageTabViewModel?.LoadPreviousImage();
                        break;
                }

                break;
            case {Key: Key.L}:
            case {Key: Key.Right}:
            case {Key: Key.PageDown}:
                switch (FileExplorerViewModel)
                {
                    case {IsFileExplorerVisible: false}:
                    case {IsFileExplorerExpanded: false}:
                        TabsViewModel.ImageTabViewModel?.LoadNextImage();
                        break;
                }

                break;
            case {Key: Key.Home}:
                switch (FileExplorerViewModel)
                {
                    case {IsFileExplorerVisible: false}:
                    case {IsFileExplorerExpanded: false}:
                        TabsViewModel.ImageTabViewModel?.LoadFirstImage();
                        break;
                }

                break;
            case {Key: Key.End}:
                switch (FileExplorerViewModel)
                {
                    case {IsFileExplorerVisible: false}:
                    case {IsFileExplorerExpanded: false}:
                        TabsViewModel.ImageTabViewModel?.LoadLastImage();
                        break;
                }

                break;
        }
    }

    private void ToggleFullScreen()
    {
        if (_mainWindow.WindowState is WindowState.Normal)
        {
            _mainWindow.WindowState = WindowState.FullScreen;
            MenuBarViewModel.IsMenuVisible = false;
            IsStatusBarVisible = false;
        }
        else
        {
            _mainWindow.WindowState = WindowState.Normal;
            MenuBarViewModel.IsMenuVisible = true;
            IsStatusBarVisible = true;
        }
    }

    private void AddTabViewModelToOpenTabs(TabViewModelBase tabViewModelBase)
    {
        // var any = OpenTabs.Any(x => x.ImagesInPath.First() == tabViewModel.ImagesInPath.First());
        // if (!any)

        TabsViewModel.AddTab(tabViewModelBase);
    }
}