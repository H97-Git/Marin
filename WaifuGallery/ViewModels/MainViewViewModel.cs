using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using ReactiveUI;
using WaifuGallery.Commands;
using WaifuGallery.Helpers;
using WaifuGallery.ViewModels.FileExplorer;
using WaifuGallery.ViewModels.Tabs;
using File = System.IO.File;

namespace WaifuGallery.ViewModels;

public class MainViewViewModel : ViewModelBase
{
    #region Public Properties

    public MenuBarViewModel MenuBarViewModel { get; init; }
    public TabsViewModel TabsViewModel { get; init; }
    public FileExplorerViewModel FileExplorerViewModel { get; init; }
    public StatusBarViewModel StatusBarViewModel { get; init; }

    #endregion

    #region CTOR

    public MainViewViewModel()
    {
        MenuBarViewModel = new MenuBarViewModel();
        TabsViewModel = new TabsViewModel();
        FileExplorerViewModel = new FileExplorerViewModel();
        StatusBarViewModel = new StatusBarViewModel();
        MessageBus.Current.Listen<ExitCommand>().Subscribe(_ => App.CloseOnExitCommand());
        MessageBus.Current.Listen<ToggleFullScreenCommand>().Subscribe(_ => ToggleFullScreen());
        MessageBus.Current.Listen<CopyCommand>().Subscribe(CopyFile);
        MessageBus.Current.Listen<CutCommand>().Subscribe(CutFile);
        MessageBus.Current.Listen<RenameCommand>().Subscribe(RenameFile);
        MessageBus.Current.Listen<PasteCommand>().Subscribe(PasteFile);
    }

    #endregion

    #region Private Methods

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
                TabsViewModel.FitToHeightAndResetZoom();
                break;
            case {Key: Key.W, KeyModifiers: KeyModifiers.Shift}:
                TabsViewModel.FitToWidthAndResetZoom();
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

        if (e.Key == Preferences.Instance.OpenSettingsKey)
        {
            if (!FileExplorerViewModel.IsFileExplorerExpandedAndVisible)
                TabsViewModel.OpenSettingsTab();
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
                FileExplorerViewModel.OpenImageTabFromKeyboardEvent();
                break;
            case {Key: Key.P}:
                var imagesInPath =
                    Helper.GetAllImagesInPath(FileExplorerViewModel.SelectedFile.FullPath);
                FileExplorerViewModel.StartPreview(imagesInPath);
                break;
            case {Key: Key.Escape}:
                FileExplorerViewModel.ClosePreview();
                break;
        }
    }

    private void ToggleFullScreen()
    {
        if (App.GetTopLevel() is not Window mainWindow) return;
        if (mainWindow.WindowState is WindowState.Normal)
        {
            mainWindow.WindowState = WindowState.FullScreen;
            MenuBarViewModel.IsMenuVisible = false;
            StatusBarViewModel.IsStatusBarVisible = false;
        }
        else
        {
            mainWindow.WindowState = WindowState.Normal;
            MenuBarViewModel.IsMenuVisible = true;
            StatusBarViewModel.IsStatusBarVisible = true;
        }
    }

    private void RenameFile(RenameCommand command)
    {
        var source = command.OldName;
        var newName = command.NewName;
        var destination = source.Replace(Path.GetFileName(source), newName);
        File.Move(source, destination);
    }


    private void CopyFile(CopyCommand command)
    {
        if (App.GetTopLevel() is Window mainWindow)
        {
            mainWindow.Clipboard?.SetTextAsync(command.Path);
        }
    }

    private void CutFile(CutCommand command)
    {
        if (App.GetTopLevel() is Window mainWindow)
        {
            mainWindow.Clipboard?.SetTextAsync(command.Path);
        }
    }

    private void PasteFile(PasteCommand command)
    {
        // switch (_lastCommandType)
        // {
        //     case CommandType.Copy:
        //         var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(command.Path);
        //         var newPath = FileExplorerViewModel.CurrentPath + "\\" + fileNameWithoutExtension;
        //         if (Directory.Exists(newPath))
        //         {
        //             newPath += " (1)";
        //         }
        //
        //         File.Copy(command.Path, newPath);
        //         
        //     case CommandType.Cut:
        //         File.Move(command.Path, FileExplorerViewModel.CurrentPath);
        //         
        // }
    }

    private void NewFolder()
    {
        Directory.CreateDirectory(Path.Combine(FileExplorerViewModel.CurrentPath, "New Folder"));
    }

    #endregion

    #region Public Methods

    public void HandleKeyBoardEvent(KeyEventArgs e)
    {
        if (FileExplorerViewModel.IsSearchFocused) return;
        HandleMainViewKeyboardEvent(e);

        if (FileExplorerViewModel.IsFileExplorerExpandedAndVisible)
            HandleFileExplorerKeyboardEvent(e);
    }

    public void HandleTabKeyEvent(KeyEventArgs e)
    {
        if (e.Key is not Key.Tab) return;
        if (e.KeyModifiers.ToString() is "Control, Shift")
        {
            TabsViewModel.CycleTab(true, true);
            e.Handled = true;
            return;
        }

        TabsViewModel.CycleTab(e.KeyModifiers is KeyModifiers.Shift, e.KeyModifiers is KeyModifiers.Control);
        e.Handled = true;
    }

    #endregion
}