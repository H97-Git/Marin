using System;
using Avalonia;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace WaifuGallery.ViewModels;

public class PreviewImageViewModel : ViewModelBase
{
    #region Private Members

    private Bitmap? _previewImage;
    private Point _previewImagePosition;
    private bool _isPreviewImageVisible;
    private int _previewImageHeight;
    private int _previewImageWidth;
    private int _previewImageIndex;
    private string[] _previewImagePaths = [];

    #endregion

    public PreviewImageViewModel()
    {
        PreviewImageWidth = PreviewImageHeight = 300;
        PreviewImagePosition = new Point(0, 0);
    }

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

    public bool IsPreviewImageVisible
    {
        get => _isPreviewImageVisible;
        set => this.RaiseAndSetIfChanged(ref _isPreviewImageVisible, value);
    }

    public int PreviewImageHeight
    {
        get => _previewImageHeight;
        set => this.RaiseAndSetIfChanged(ref _previewImageHeight, value);
    }

    public int PreviewImageWidth
    {
        get => _previewImageWidth;
        set => this.RaiseAndSetIfChanged(ref _previewImageWidth, value);
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

        _previewImageIndex = 0;
        PreviewImage = new Bitmap(_previewImagePaths[_previewImageIndex]);
        IsPreviewImageVisible = true;
    }

    public void NextPreview()
    {
        _previewImageIndex = Math.Min(_previewImagePaths.Length - 1, _previewImageIndex + 1);
        PreviewImage = new Bitmap(_previewImagePaths[_previewImageIndex]);
    }

    public void PreviousPreview()
    {
        _previewImageIndex = Math.Max(0, _previewImageIndex - 1);
        PreviewImage = new Bitmap(_previewImagePaths[_previewImageIndex]);
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
        if (newDelta < 0)
        {
            PreviewImageWidth = Math.Max(200, PreviewImageWidth + newDelta);
            PreviewImageHeight = Math.Max(200, PreviewImageHeight + newDelta);
        }
        else
        {
            PreviewImageWidth = Math.Min(600, PreviewImageWidth + newDelta);
            PreviewImageHeight = Math.Min(600, PreviewImageHeight + newDelta);
        }
    }

    #endregion
}