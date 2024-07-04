using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using WaifuGallery.Commands;
using WaifuGallery.Helpers;
using WaifuGallery.ViewModels.FileExplorer;

namespace WaifuGallery.Controls;

public partial class File : UserControl
{
    #region Private Fields

    private FileViewModel? FileViewModel => DataContext as FileViewModel;
    private readonly DispatcherTimer _previewTimer;

    #endregion

    #region Private Methods

    private void OnFolderDoubleClick_ChangePath(object? sender, TappedEventArgs e)
    {
        StopTimer();
        if (FileViewModel is null) return;
        var command = new ChangePathCommand(FileViewModel.FullPath);
        MessageBus.Current.SendMessage(command);
    }

    private void OnPointerPressed_StartTimerTickPreview(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed) return;
        if (FileViewModel is {IsImage: true}) return;
        _previewTimer.Start();
    }

    /// <summary>
    /// Setup the preview and start it.
    /// </summary>
    /// <param name="sender">Unused</param>
    /// <param name="e">Unused</param>
    private void OnPreviewTimerTick(object? sender, EventArgs e)
    {
        if (!_previewTimer.IsEnabled) return;
        if (FileViewModel is null) return;
        _previewTimer.Stop();
        var command = new StartPreviewCommand(FileViewModel.FullPath);
        MessageBus.Current.SendMessage(command);
    }

    // private void OnPointerExited_ClosePreview(object? sender, PointerEventArgs e) =>
    //     FileViewModel?.SendCommandToFileExplorer(new Command(CommandType.ClosePreview));

    private void OnPointerReleased_OpenInNewTab(object? sender, PointerReleasedEventArgs e)
    {
        StopTimer();
        if (e.InitialPressMouseButton is not MouseButton.Middle) return;
        if (FileViewModel is null) return;

        var imagesInPath = Helper.GetAllImagesInPath(FileViewModel);
        if (imagesInPath is {Length: 0})
        {
            MessageBus.Current.SendMessage(new SendMessageToStatusBarCommand(InfoBarSeverity.Warning,
                "No images found in folder"));
            return;
        }

        var index = 0;
        if (FileViewModel.IsImage)
        {
            index = Array.IndexOf(imagesInPath, FileViewModel.FullPath);
        }

        var command = new OpenInNewTabCommand(index, imagesInPath);

        MessageBus.Current.SendMessage(command);
    }

    private void StopTimer()
    {
        if (_previewTimer.IsEnabled)
            _previewTimer.Stop();
    }


    private void Rename_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (FileViewModel is null) return;
        if (e.Key is Key.Escape) FileViewModel.IsRenaming = false;
        if (e.Key is not Key.Enter) return;
        var command = new RenameCommand(FileViewModel.FullPath, FileViewModel.FileName);
        MessageBus.Current.SendMessage(command);
        FileViewModel.IsRenaming = false;
        e.Handled = true;
    }

    #endregion

    #region CTOR

    public File()
    {
        _previewTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(.5),
        };
        _previewTimer.Tick += OnPreviewTimerTick;
        InitializeComponent();
        FileControl.DoubleTapped += OnFolderDoubleClick_ChangePath;
        // FileControl.PointerExited += OnPointerExited_ClosePreview;
        FileControl.PointerPressed += OnPointerPressed_StartTimerTickPreview;
        FileControl.PointerReleased += OnPointerReleased_OpenInNewTab;
    }

    #endregion

    protected override void OnLoaded(RoutedEventArgs e)
    {
        if(DataContext is FileViewModel fileViewModel)
            fileViewModel.GetThumbnail(fileViewModel._fileSystemInfo as FileInfo);
        base.OnLoaded(e);
    }
}