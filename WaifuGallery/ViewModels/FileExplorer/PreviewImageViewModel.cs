using System;
using Avalonia;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace WaifuGallery.ViewModels.FileExplorer;

public class PreviewImageViewModel : ViewModelBase
{
    #region Private Members

    private Bitmap? _previewImage;
    private Point _previewImagePosition;
    private bool _isPreviewImageVisible;
    private Size _previewImageSize;
    private int _previewImageIndex;
    private string[] _previewImagePaths = [];

    #endregion

    public PreviewImageViewModel()
    {
        PreviewImageSize = new Size(300, 300);
        PreviewImagePosition = new Point(0, 0);
    }

    #region Public Properties

    public Bitmap? PreviewImage
    {
        get => _previewImage;
        set => this.RaiseAndSetIfChanged(ref _previewImage, value);
    }

    public int PreviewImageIndex
    {
        get => _previewImageIndex;
        set
        {
            if (value < 0)
                value = 0;
            if (value >= _previewImagePaths.Length)
                value = _previewImagePaths.Length - 1;
            this.RaiseAndSetIfChanged(ref _previewImageIndex, value);
        }
    }

    public Point PreviewImagePosition
    {
        get => _previewImagePosition;
        private set => this.RaiseAndSetIfChanged(ref _previewImagePosition, value);
    }

    public bool IsPreviewImageVisible
    {
        get => _isPreviewImageVisible;
        set => this.RaiseAndSetIfChanged(ref _isPreviewImageVisible, value);
    }

    public Size PreviewImageSize
    {
        get => _previewImageSize;
        set => this.RaiseAndSetIfChanged(ref _previewImageSize, value);
    }

    #endregion

    #region Private Methods

    public void StartPreview(string[] imagesInPath)
    {
        _previewImagePaths = imagesInPath;
        if (_previewImagePaths is {Length: 0})
        {
            // StatusBarMessage = "No images found for preview";
            return;
        }

        PreviewImageIndex = 0;
        PreviewImage = new Bitmap(_previewImagePaths[PreviewImageIndex]);
        IsPreviewImageVisible = true;
    }

    public void NextPreview()
    {
        PreviewImageIndex += 1;
        PreviewImage = new Bitmap(_previewImagePaths[PreviewImageIndex]);
    }

    public void PreviousPreview()
    {
        PreviewImageIndex -= 1;
        PreviewImage = new Bitmap(_previewImagePaths[PreviewImageIndex]);
    }

    public void ClosePreview()
    {
        if (!IsPreviewImageVisible) return;
        PreviewImage = null;
        IsPreviewImageVisible = false;
        _previewImagePaths = Array.Empty<string>();
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
            newWidth = Math.Max(200, PreviewImageSize.Width + newDelta);
            newHeight = Math.Max(200, PreviewImageSize.Height + newDelta);
        }
        else
        {
            newWidth = Math.Min(600, PreviewImageSize.Width + newDelta);
            newHeight = Math.Min(600, PreviewImageSize.Height + newDelta);
        }

        PreviewImageSize = new Size(newWidth, newHeight);
    }

    #endregion
}