using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using WaifuGallery.Commands;
using WaifuGallery.Controls.Dialogs;
using WaifuGallery.ViewModels.Dialogs;
using WaifuGallery.ViewModels.FileExplorer;
using WaifuGallery.ViewModels.Tabs;
using WaifuGallery.Helpers;
using WaifuGallery.Models;
using File = System.IO.File;

namespace WaifuGallery.ViewModels;

public class MainViewViewModel : ViewModelBase
{
    #region Private Fields

    private bool _isDialogOpen;
    private readonly DataObject _clipBoardDataObject = new();
    private ImageTabViewModel? ImageTabViewModel => TabsViewModel.ImageTabViewModel;
    private PreviewImageViewModel PreviewImageViewModel => FileExplorerViewModel.PreviewImageViewModel;

    #endregion

    #region Private Methods

    private void HandleMainViewKeyboardEvent(KeyEventArgs e)
    {
        var keyGesture = new KeyGesture(e.Key, e.KeyModifiers);
        var hk = Settings.Instance.HotKeyManager.GetBinding(keyGesture);
        switch (hk)
        {
            case KeyCommand.FirstImage:
                if (!FileExplorerViewModel.IsFileExplorerExpandedAndVisible)
                    TabsViewModel.ImageTabViewModel?.LoadFirstImage();
                break;
            case KeyCommand.LastImage:
                if (!FileExplorerViewModel.IsFileExplorerExpandedAndVisible)
                    TabsViewModel.ImageTabViewModel?.LoadLastImage();
                break;
            case KeyCommand.GoRight:
            case KeyCommand.NextImage:
                if (!FileExplorerViewModel.IsFileExplorerExpandedAndVisible)
                    TabsViewModel.ImageTabViewModel?.LoadNextImage();
                break;
            case KeyCommand.PreviousImage:
            case KeyCommand.GoLeft:
                if (!FileExplorerViewModel.IsFileExplorerExpandedAndVisible)
                    TabsViewModel.ImageTabViewModel?.LoadPreviousImage();
                break;
            case KeyCommand.FullScreen:
                ToggleFullScreen();
                break;
            case KeyCommand.FitToWidthAndResetZoom:
                TabsViewModel.FitToWidthAndResetZoom();
                break;
            case KeyCommand.FitToHeightAndResetZoom:
                TabsViewModel.FitToHeightAndResetZoom();
                break;
            case KeyCommand.OpenPreferences:
                if (!FileExplorerViewModel.IsFileExplorerExpandedAndVisible)
                    TabsViewModel.OpenSettingsTab();
                break;
            case KeyCommand.None:
                return;
            default:
                return;
        }
    }

    private void HandleFileExplorerKeyboardEvent(KeyEventArgs e)
    {
        var keyGesture = new KeyGesture(e.Key, e.KeyModifiers);
        var hk = Settings.Instance.HotKeyManager.GetBinding(keyGesture);
        switch (hk)
        {
            case KeyCommand.NextImage:
                PreviewImageViewModel.NextPreview();
                break;
            case KeyCommand.PreviousImage:
                PreviewImageViewModel.NextPreview();
                break;
            case KeyCommand.GoUp:
                FileExplorerViewModel.GoUp();
                break;
            case KeyCommand.GoDown:
                FileExplorerViewModel.GoDown();
                break;
            case KeyCommand.GoLeft:
                FileExplorerViewModel.SelectedIndex -=1;
                break;
            case KeyCommand.GoRight:
                FileExplorerViewModel.SelectedIndex +=1;
                break;
            case KeyCommand.GoToParentFolder:
                FileExplorerViewModel.GoToParentFolder();
                break;
            case KeyCommand.OpenFolder:
                FileExplorerViewModel.ChangePath(FileExplorerViewModel.SelectedFile.FullPath);
                break;
            case KeyCommand.OpenImageInNewTab:
                FileExplorerViewModel.OpenImageTabFromKeyboardEvent();
                break;
            case KeyCommand.ToggleFileExplorer:
                FileExplorerViewModel.ToggleFileExplorer();
                break;
            case KeyCommand.ToggleFileExplorerVisibility:
                FileExplorerViewModel.ToggleFileExplorerVisibility();
                break;
            case KeyCommand.ShowPreview:
                PreviewImageViewModel.ShowPreview(FileExplorerViewModel.SelectedFile.FullPath);
                break;
            case KeyCommand.HidePreview:
                PreviewImageViewModel.HidePreview();
                break;
            case KeyCommand.None:
                break;
            default:
                return;
        }
    }

    private void ToggleFullScreen()
    {
        if (App.GetTopLevel() is not Window mainWindow) return;
        if (mainWindow.WindowState is WindowState.Normal)
        {
            mainWindow.WindowState = WindowState.FullScreen;
            if (Settings.Instance.ShouldHideMenuBar)
                MenuBarViewModel.IsMenuVisible = false;
            if (Settings.Instance.ShouldHideTabsHeader)
                TabsViewModel.IsTabHeadersVisible = false;
            if (Settings.Instance.ShouldHideFileExplorer)
                FileExplorerViewModel.IsFileExplorerVisible = false;
            if (Settings.Instance.ShouldHideStatusBar)
                StatusBarViewModel.IsStatusBarVisible = false;
        }
        else
        {
            mainWindow.WindowState = WindowState.Normal;
            MenuBarViewModel.IsMenuVisible = true;
            TabsViewModel.IsTabHeadersVisible = true;
            FileExplorerViewModel.IsFileExplorerVisible = true;
            StatusBarViewModel.IsStatusBarVisible = true;
        }
    }

    private void RenameFile(RenameCommand command)
    {
        var source = command.Path;
        var destination = source.Replace(Path.GetFileName(source), command.NewName);
        if (!File.Exists(source) && !Directory.Exists(source))
        {
            SendMessageToStatusBar(InfoBarSeverity.Error, "File or Directory does not exist!");
            return;
        }

        try
        {
            Directory.Move(source, destination);
        }
        catch (Exception e)
        {
            SendMessageToStatusBar(InfoBarSeverity.Error, "Failed to rename: " + e.Message);
            return;
        }

        SendMessageToStatusBar(InfoBarSeverity.Success, "File renamed successfully!");
    }

    private void CopyFile(CopyCommand command)
    {
        if (App.GetTopLevel()?.Clipboard is not { } clipboard) return;
        _clipBoardDataObject.Set(DataFormats.FileNames, command);
        clipboard.SetTextAsync(command.Path);
    }

    private void CutFile(CutCommand command)
    {
        if (App.GetTopLevel()?.Clipboard is not { } clipboard) return;
        _clipBoardDataObject.Set(DataFormats.FileNames, command);
        clipboard.SetTextAsync(command.Path);
    }

    private void DeleteFile(DeleteCommand command)
    {
        var path = command.Path;
        if (!File.Exists(path) && !Directory.Exists(path))
        {
            SendMessageToStatusBar(InfoBarSeverity.Warning, "File or Directory does not exist!");
            return;
        }

        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            else
            {
                File.Delete(path);
            }
        }
        catch (Exception e)
        {
            SendMessageToStatusBar(InfoBarSeverity.Error, "Failed to delete: " + e.Message);
            return;
        }

        MessageBus.Current.SendMessage(new RefreshFileExplorerCommand());
        SendMessageToStatusBar(InfoBarSeverity.Success, "File deleted successfully!");
    }

    private void PasteFile(PasteCommand command)
    {
        var sourceFileCommand = _clipBoardDataObject.Get(DataFormats.FileNames) as FileCommand;
        var destination = command.Path;
        switch (sourceFileCommand)
        {
            case CopyCommand:
                if (File.Exists(sourceFileCommand.Path))
                {
                    if (sourceFileCommand.Path == destination)
                    {
                        destination += "_copy";
                    }

                    File.Copy(sourceFileCommand.Path, destination);
                }
                else
                {
                    Helper.CopyDirectory(sourceFileCommand.Path, destination, true);
                }

                break;
            case CutCommand:
                if (File.Exists(sourceFileCommand.Path))
                {
                    File.Move(sourceFileCommand.Path, destination);
                }
                else
                {
                    var s = Path.Combine(destination, Path.GetFileName(sourceFileCommand.Path));
                    Directory.Move(sourceFileCommand.Path, s);
                }

                break;
        }

        MessageBus.Current.SendMessage(new RefreshFileExplorerCommand());
        SendMessageToStatusBar(InfoBarSeverity.Success, "Pasted successfully!");
    }

    private async void NewFolder(NewFolderCommand command)
    {
        var isCanceled = false;
        try
        {
            _isDialogOpen = true;
            var newFolderName = await ShowNewFolderDialogAsync();
            if (newFolderName != null)
            {
                Directory.CreateDirectory(Path.Combine(command.Path, newFolderName));
            }
            else
            {
                isCanceled = true;
            }
        }
        catch (Exception e)
        {
            SendMessageToStatusBar(InfoBarSeverity.Error, "Failed to create folder: " + e.Message);
            return;
        }
        finally
        {
            _isDialogOpen = false;
        }

        if (isCanceled) return;
        MessageBus.Current.SendMessage(new RefreshFileExplorerCommand());
        SendMessageToStatusBar(InfoBarSeverity.Success, "Folder created successfully!");
    }

    private async Task<string?> ShowNewFolderDialogAsync()
    {
        var dialog = new ContentDialog()
        {
            Title = "New folder name:",
            PrimaryButtonText = "Ok",
            SecondaryButtonText = "Cancel",
        };

        var viewModel = new NewFolderViewModel();
        dialog.Content = new NewFolder()
        {
            DataContext = viewModel,
            OnEnterPressed = (_, _) => { dialog.Hide(); },
            OnEscapePressed = (_, _) => { dialog.Hide(); }
        };

        var dialogResult = await dialog.ShowAsync();
        return dialogResult is ContentDialogResult.Secondary ? null : viewModel.NewFolderName;
    }

    private void OpenInFileExplorer(OpenInFileExplorerCommand command)
    {
        Process.Start("explorer.exe", command.Path);
    }

    private void OpenInBrowser(OpenInBrowserCommand command)
    {
        var vivaldiPath = Process.GetProcessesByName("vivaldi").FirstOrDefault()?.MainModule?.FileName;
        if (vivaldiPath == null) return;
        var fileUri = new Uri(command.Path);
        Process.Start(vivaldiPath, fileUri.ToString());
    }

    private void ExtractFile(ExtractCommand command)
    {
        Helper.ExtractDirectory(command.Path);
    }

    private void ShowPreview()
    {
        FileExplorerViewModel.PreviewImageViewModel.ShowPreview(FileExplorerViewModel.SelectedFile.FullPath);
    }

    private void ChangePath()
    {
        FileExplorerViewModel.ChangePath(FileExplorerViewModel.SelectedFile.FullPath);
    }

    private void GoLeft()
    {
        if (FileExplorerViewModel.PreviewImageViewModel.IsPreviewImageVisible)
        {
            FileExplorerViewModel.PreviewImageViewModel.PreviousPreview();
        }
        else
        {
            FileExplorerViewModel.SelectedIndex -= 1;
        }
    }

    private void GoRight()
    {
        if (FileExplorerViewModel.PreviewImageViewModel.IsPreviewImageVisible)
        {
            FileExplorerViewModel.PreviewImageViewModel.NextPreview();
        }
        else
        {
            FileExplorerViewModel.SelectedIndex += 1;
        }
    }

    #endregion

    #region CTOR

    public MainViewViewModel()
    {
        MenuBarViewModel = new MenuBarViewModel();
        TabsViewModel = new TabsViewModel();
        FileExplorerViewModel = new FileExplorerViewModel();
        StatusBarViewModel = new StatusBarViewModel();
        MessageBus.Current.Listen<ClearCacheCommand>().Subscribe(_ => Helper.ClearThumbnailsCache());
        MessageBus.Current.Listen<CopyCommand>().Subscribe(CopyFile);
        MessageBus.Current.Listen<CutCommand>().Subscribe(CutFile);
        MessageBus.Current.Listen<DeleteCommand>().Subscribe(DeleteFile);
        MessageBus.Current.Listen<ExitCommand>().Subscribe(_ => App.CloseOnExitCommand());
        MessageBus.Current.Listen<ExtractCommand>().Subscribe(ExtractFile);
        MessageBus.Current.Listen<NewFolderCommand>().Subscribe(NewFolder);
        MessageBus.Current.Listen<OpenInBrowserCommand>().Subscribe(OpenInBrowser);
        MessageBus.Current.Listen<OpenInFileExplorerCommand>().Subscribe(OpenInFileExplorer);
        MessageBus.Current.Listen<PasteCommand>().Subscribe(PasteFile);
        MessageBus.Current.Listen<RenameCommand>().Subscribe(RenameFile);
        MessageBus.Current.Listen<ToggleFullScreenCommand>().Subscribe(_ => ToggleFullScreen());
    }

    #endregion

    #region Public Properties

    public MenuBarViewModel MenuBarViewModel { get; set; }
    public TabsViewModel TabsViewModel { get; set; }

    public FileExplorerViewModel FileExplorerViewModel { get; set; }
    public StatusBarViewModel StatusBarViewModel { get; }

    #endregion

    #region Public Methods

    public void HandleKeyBoardEvent(KeyEventArgs e)
    {
        if (_isDialogOpen) return;
        if (FileExplorerViewModel.IsSearchFocused) return;
        HandleMainViewKeyboardEvent(e);

        if (FileExplorerViewModel.IsFileExplorerExpandedAndVisible)
            HandleFileExplorerKeyboardEvent(e);
        e.Handled = true;
    }

    public void HandleTabKeyEvent(KeyEventArgs e)
    {
        if (e.Key is not Key.Tab) return;
        e.Handled = true;
        if (e.KeyModifiers.ToString() is "Control, Shift")
        {
            TabsViewModel.CycleTab(true, true);
            return;
        }

        TabsViewModel.CycleTab(e.KeyModifiers is KeyModifiers.Shift, e.KeyModifiers is KeyModifiers.Control);
    }

    #endregion
}