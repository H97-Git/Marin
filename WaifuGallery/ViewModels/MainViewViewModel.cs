using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using WaifuGallery.Controls;
using WaifuGallery.Helpers;
using WaifuGallery.Models;
using WaifuGallery.ViewModels.FileExplorer;
using WaifuGallery.ViewModels.Tabs;
using File = System.IO.File;

namespace WaifuGallery.ViewModels;

public class MainViewViewModel : ViewModelBase
{
    #region Private Members

    private readonly MainWindow _mainWindow;
    private CommandType _lastCommandType = CommandType.None;

    #endregion

    #region Public Properties

    public MenuBarViewModel MenuBarViewModel { get; init; }
    public TabsViewModel TabsViewModel { get; init; }
    public FileExplorerViewModel FileExplorerViewModel { get; init; }
    public StatusBarViewModel StatusBarViewModel { get; init; }

    #endregion

    #region CTOR

    public MainViewViewModel(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        MenuBarViewModel = new MenuBarViewModel();
        TabsViewModel = new TabsViewModel();
        FileExplorerViewModel = new FileExplorerViewModel(_mainWindow.StorageProvider);
        StatusBarViewModel = new StatusBarViewModel();
        AddEventHandlers();
    }

    #endregion

    #region Private Methods

    private void AddEventHandlers()
    {
        MenuBarViewModel.OnSendCommandToMainView += HandleControlCommands;
        FileExplorerViewModel.OnSendCommandToMainView += HandleControlCommands;
        TabsViewModel.OnSendCommandToMainView += HandleControlCommands;
    }

    private void HandleControlCommands(object? sender, Command command)
    {
        if (sender is null) return;
        switch (sender.GetType().Name)
        {
            case "MenuBarViewModel":
                HandleMenuBarCommand(command.Type);
                break;
            case "FileExplorerViewModel":
                HandleFileExplorerCommand(command);
                break;
        }
    }

    private void HandleMenuBarCommand(CommandType type)
    {
        switch (type)
        {
            case CommandType.Exit:
                Environment.Exit(0);
                break;
            case CommandType.ToggleFullScreen:
                StatusBarViewModel.Message = $"Toggling FullScreen...";
                ToggleFullScreen();
                break;
            case CommandType.OpenFile:
                StatusBarViewModel.Message = $"Opening File...";
                break;
            case CommandType.ToggleFileExplorer:
                StatusBarViewModel.Message = $"Toggling File Explorer...";
                FileExplorerViewModel.ToggleFileExplorer();
                break;
            case CommandType.ToggleFileExplorerVisibility:
                StatusBarViewModel.Message = $"Toggling File Explorer Visibility...";
                FileExplorerViewModel.ToggleFileExplorerVisibility();
                break;
            case CommandType.FitToWidth:
                StatusBarViewModel.Message = $"FitToWidth";
                TabsViewModel.FitToHeight();
                break;
            case CommandType.FitToHeight:
                StatusBarViewModel.Message = $"FitToHeight";
                TabsViewModel.FitToWidth();
                break;
            default:
                StatusBarViewModel.Message = "Command not found!";
                break;
        }
    }

    private void HandleFileExplorerCommand(Command command)
    {
        switch (command.Type)
        {
            case CommandType.Copy:
            case CommandType.Cut:
                CopyCutFile(command);
                break;
            case CommandType.Rename:
                RenameFile(command);
                break;
            case CommandType.Paste:
                PasteFile(command);
                break;
            case CommandType.Delete:
                DeleteFile();
                break;
            case CommandType.OpenImageInNewTab:
            case CommandType.OpenFolderInNewTab:
                TabsViewModel.AddImageTab(command);
                break;
            case CommandType.SendMessageToStatusBar:
                if (command.Message is not null)
                    StatusBarViewModel.Message = command.Message;
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
                TabsViewModel.FitToHeight();
                break;
            case {Key: Key.W, KeyModifiers: KeyModifiers.Shift}:
                TabsViewModel.FitToWidth();
                break;
            case {Key: Key.Tab, KeyModifiers: KeyModifiers.Control}:
                if (!FileExplorerViewModel.IsFileExplorerExpandedAndVisible)
                    TabsViewModel.SwitchTab();
                break;
            case {Key: Key.H}:
            case {Key: Key.Left}:
            case {Key: Key.PageUp}:
                if (!FileExplorerViewModel.IsFileExplorerExpandedAndVisible)
                    TabsViewModel.ImageTabViewModel?.LoadPreviousImage();
                break;
            case {Key: Key.L}:
            case {Key: Key.Right}:
            case {Key: Key.PageDown}:
                if (!FileExplorerViewModel.IsFileExplorerExpandedAndVisible)
                    TabsViewModel.ImageTabViewModel?.LoadNextImage();
                break;
            case {Key: Key.Home}:
                if (!FileExplorerViewModel.IsFileExplorerExpandedAndVisible)
                    TabsViewModel.ImageTabViewModel?.LoadFirstImage();
                break;
            case {Key: Key.End}:
                if (!FileExplorerViewModel.IsFileExplorerExpandedAndVisible)
                    TabsViewModel.ImageTabViewModel?.LoadLastImage();
                break;
            case {Key: Key.P, KeyModifiers: KeyModifiers.Control}:
            case {Key: Key.OemComma, KeyModifiers: KeyModifiers.Control}:
                if (!FileExplorerViewModel.IsFileExplorerExpandedAndVisible)
                    TabsViewModel.OpenSettingsTab();
                break;
        }
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
                FileExplorerViewModel.OpenImageTab();
                break;
            case {Key: Key.P}:
                var imagesInPath =
                    Helper.GetAllImagesInPathFromString(FileExplorerViewModel.SelectedFile.FullPath);
                FileExplorerViewModel.StartPreview(imagesInPath);
                break;
            case {Key: Key.Escape}:
                FileExplorerViewModel.ClosePreview();
                break;
        }
    }

    private void ToggleFullScreen()
    {
        if (_mainWindow.WindowState is WindowState.Normal)
        {
            _mainWindow.WindowState = WindowState.FullScreen;
            MenuBarViewModel.IsMenuVisible = false;
            StatusBarViewModel.IsStatusBarVisible = false;
        }
        else
        {
            _mainWindow.WindowState = WindowState.Normal;
            MenuBarViewModel.IsMenuVisible = true;
            StatusBarViewModel.IsStatusBarVisible = true;
        }
    }

    private void RenameFile(Command command)
    {
        var newName = command.Message;
        var source = command.Path;
        var destination = source.Replace(Path.GetFileName(source), newName);
        File.Move(source, destination);
    }

    private void DeleteFile()
    {
        File.Delete(FileExplorerViewModel.SelectedFile.FullPath);
    }

    private void CopyCutFile(Command command)
    {
        _mainWindow.Clipboard?.SetTextAsync(command.Path);
        _lastCommandType = command.Type;
    }

    private void PasteFile(Command command)
    {
        switch (_lastCommandType)
        {
            case CommandType.Copy:
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(command.Path);
                var newPath = FileExplorerViewModel.CurrentPath + "\\" + fileNameWithoutExtension;
                if (Directory.Exists(newPath))
                {
                    newPath += " (1)";
                }

                File.Copy(command.Path, newPath);
                break;
            case CommandType.Cut:
                File.Move(command.Path, FileExplorerViewModel.CurrentPath);
                break;
        }
    }

    private void NewFolder()
    {
        Directory.CreateDirectory(Path.Combine(FileExplorerViewModel.CurrentPath, "New Folder"));
    }

    #endregion

    #region Public Methods

    public void HandleKeyboardEvent(KeyEventArgs e)
    {
        if (FileExplorerViewModel.IsSearchFocused) return;
        HandleMainViewKeyboardEvent(e);

        if (FileExplorerViewModel.IsFileExplorerExpandedAndVisible)
            HandleFileExplorerKeyboardEvent(e);
    }

    #endregion
}