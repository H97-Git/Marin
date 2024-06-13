using System;
using Avalonia;
using Avalonia.Media.Imaging;
using ReactiveUI;
using WaifuGallery.Models;

namespace WaifuGallery.ViewModels.FileExplorer;

public class PreviewImageViewModel : ViewModelBase
{
    #region Private Members

    private Bitmap? _previewImage;
    private Point _previewImagePosition;
    private Size _previewImageSize;
    private bool _isPreviewImageVisible;
    private int _previewImageIndex;
    private string[] _previewImagePaths = [];

    private int PreviewImageIndex
    {
        get => _previewImageIndex;
        set
        {
            // A potential to let user loop through images in preview.
            if (value < 0)
                value = 0;
            if (value >= _previewImagePaths.Length)
                value = _previewImagePaths.Length - 1;
            _previewImageIndex = value;
            PreviewImage = new Bitmap(_previewImagePaths[PreviewImageIndex]);
        }
    }

    #endregion

    #region CTOR

    public PreviewImageViewModel()
    {
        PreviewImageSize = new Size(300, 300);
        PreviewImagePosition = new Point(0, 0);
    }

    #endregion

    #region Public Events

    public event EventHandler<Command>? OnSendCommandToFileExplorer;

    #endregion

    #region Public Properties

    public Bitmap? PreviewImage
    {
        get => _previewImage;
        set => this.RaiseAndSetIfChanged(ref _previewImage, value);
    }

    public Point PreviewImagePosition
    {
        get => _previewImagePosition;
        private set => this.RaiseAndSetIfChanged(ref _previewImagePosition, value);
    }

    public Size PreviewImageSize
    {
        get => _previewImageSize;
        private set => this.RaiseAndSetIfChanged(ref _previewImageSize, value);
    }

    public bool IsPreviewImageVisible
    {
        get => _isPreviewImageVisible;
        set => this.RaiseAndSetIfChanged(ref _isPreviewImageVisible, value);
    }

    #endregion

    #region Private Methods

    private void SendCommandToFileExplorer(Command command)
    {
        OnSendCommandToFileExplorer?.Invoke(this, command);
    }

    public void StartPreview(string[] imagesInPath)
    {
        _previewImagePaths = imagesInPath;
        if (_previewImagePaths is {Length: 0})
        {
            var command = new Command(CommandType.SendMessageToStatusBar, message: "No images found for preview");
            SendCommandToFileExplorer(command);
            return;
        }

        PreviewImageIndex = 0;
        IsPreviewImageVisible = true;
    }

    public void NextPreview()
    {
        PreviewImageIndex += 1;
    }

    public void PreviousPreview()
    {
        PreviewImageIndex -= 1;
    }

    public void ChangePreviewPosition(Point point)
    {
        // if (IsPreviewImageVisible) return;
        PreviewImagePosition = new Point(point.X, point.Y);
    }

    public void ZoomPreview(double deltaY)
    {
        if (!IsPreviewImageVisible) return;
        var newDelta = (int) deltaY * 20;
        double newWidth;
        double newHeight;
        if (newDelta < 0)
        {
            newWidth = Math.Max(150, PreviewImageSize.Width + newDelta);
            newHeight = Math.Max(150, PreviewImageSize.Height + newDelta);
        }
        else
        {
            newWidth = Math.Min(600, PreviewImageSize.Width + newDelta);
            newHeight = Math.Min(600, PreviewImageSize.Height + newDelta);
        }

        PreviewImageSize = new Size(newWidth, newHeight);
    }

    public void ClosePreview()
    {
        if (!IsPreviewImageVisible) return;
        IsPreviewImageVisible = false;
        PreviewImage = null;
        _previewImagePaths = Array.Empty<string>();
        SendCommandToFileExplorer(new Command(CommandType.ToggleFileExplorerScrollBar));
    }

    #endregion
}