using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using WaifuGallery.Helpers;
using WaifuGallery.Models;
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
        var command = new Command(CommandType.ChangePath, path: FileViewModel?.FullPath);
        FileViewModel?.SendCommandToFileExplorer(command);
    }

    private void OnPointerPressed_StartTimerTickPreview(object? sender, PointerPressedEventArgs e)
    {
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
        if (FileViewModel == null) return;
        var imagesInPath = ImagesHelper.GetAllImagesInPathFromFileViewModel(FileViewModel);
        if (imagesInPath is {Length: 0}) return;
        var command = new Command(CommandType.StartPreview, path: FileViewModel?.FullPath, imagesInPath: imagesInPath);
        _previewTimer.Stop();
        FileViewModel?.SendCommandToFileExplorer(command);
    }

    // private void OnPointerExited_ClosePreview(object? sender, PointerEventArgs e) =>
    //     FileViewModel?.SendCommandToFileExplorer(new Command(CommandType.ClosePreview));

    private void OnPointerReleased_OpenInNewTab(object? sender, PointerReleasedEventArgs e)
    {
        StopTimer();
        if (e.InitialPressMouseButton is not MouseButton.Middle) return;
        if (FileViewModel == null) return;

        var imagesInPath = ImagesHelper.GetAllImagesInPathFromFileViewModel(FileViewModel);
        if (imagesInPath is {Length: 0}) return;

        Command command;
        if (FileViewModel.IsImage)
        {
            var index = Array.IndexOf(imagesInPath, FileViewModel.FullPath);
            command = new Command(CommandType.OpenImageInNewTab, imagesInPath, index: index);
        }
        else
        {
            command = new Command(CommandType.OpenFolderInNewTab, imagesInPath);
        }

        FileViewModel?.SendCommandToFileExplorer(command);
    }

    private void StopTimer()
    {
        if (_previewTimer.IsEnabled)
            _previewTimer.Stop();
    }

    #endregion
}