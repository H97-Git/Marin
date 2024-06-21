using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using WaifuGallery.Commands;
using WaifuGallery.Helpers;
using WaifuGallery.ViewModels.FileExplorer;

namespace WaifuGallery.Controls;

public partial class File : UserControl
{
    #region Private Members

    private FileViewModel? FileViewModel => DataContext as FileViewModel;
    private readonly DispatcherTimer _previewTimer;

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

    #region Private Methods

    private void OnFolderDoubleClick_ChangePath(object? sender, TappedEventArgs e)
    {
        StopTimer();
        var command = new ChangePathCommand(FileViewModel?.FullPath);
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
        var imagesInPath = Helper.GetAllImagesInPath(FileViewModel);
        if (imagesInPath is {Length: 0}) return;
        var command = new StartPreviewCommand(imagesInPath);
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

    #endregion
}