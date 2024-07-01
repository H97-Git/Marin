using System;
using System.Diagnostics;
using System.IO;
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
using File = System.IO.File;

namespace WaifuGallery.ViewModels;

public class MainViewViewModel : ViewModelBase
{
    private readonly DataObject _clipBoardDataObject = new();
    private bool _isDialogOpen;

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

        if (e.Key == Settings.Instance.OpenSettingsKey)
        {
            if (!FileExplorerViewModel.IsFileExplorerExpandedAndVisible)
                TabsViewModel.OpenSettingsTab();
        }
    }

    private void HandleFileExplorerKeyboardEvent(KeyEventArgs e)
    {
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
                FileExplorerViewModel.StartPreview(FileExplorerViewModel.SelectedFile.FullPath);
                break;
            case {Key: Key.Escape}:
                if (!FileExplorerViewModel.PreviewImageViewModel.IsPreviewImageVisible) return;
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

    #endregion

    #region CTOR

    public MainViewViewModel()
    {
        MessageBus.Current.Listen<ExitCommand>().Subscribe(_ => App.CloseOnExitCommand());
        MessageBus.Current.Listen<ToggleFullScreenCommand>().Subscribe(_ => ToggleFullScreen());
        MessageBus.Current.Listen<CopyCommand>().Subscribe(CopyFile);
        MessageBus.Current.Listen<CutCommand>().Subscribe(CutFile);
        MessageBus.Current.Listen<RenameCommand>().Subscribe(RenameFile);
        MessageBus.Current.Listen<PasteCommand>().Subscribe(PasteFile);
        MessageBus.Current.Listen<DeleteCommand>().Subscribe(DeleteFile);
        MessageBus.Current.Listen<NewFolderCommand>().Subscribe(NewFolder);
        MessageBus.Current.Listen<OpenInFileExplorerCommand>().Subscribe(OpenInFileExplorer);
    }

    #endregion

    #region Public Properties

    public MenuBarViewModel MenuBarViewModel { get; set; } = new();
    public TabsViewModel TabsViewModel { get; set; } = new();
    public FileExplorerViewModel FileExplorerViewModel { get; set; } = new();
    public StatusBarViewModel StatusBarViewModel { get; } = new();

    #endregion

    #region Public Methods

    public void HandleKeyBoardEvent(KeyEventArgs e)
    {
        if (_isDialogOpen) return;
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