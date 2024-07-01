using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using WaifuGallery.Helpers;

namespace WaifuGallery.ViewModels.FileExplorer;

public class PreviewImageViewModel : ViewModelBase
{
    #region Private Fields

    private Bitmap? _previewImage;
    private Point _previewPosition = new(0, 0);
    private Size _previewSize = new(DefaultPreviewSize, DefaultPreviewSize);
    private bool _isPreviewVisible;
    private int _previewImageIndex;
    private const int DefaultPreviewSize = 300;
    private string[] _previewImagePaths = [];
    private string _previewImageCounter = "0/0";

    #endregion

    #region Private Properties

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
            PreviewCounter = $"{_previewImageIndex + 1}/{_previewImagePaths.Length}";
            PreviewImage = new Bitmap(_previewImagePaths[_previewImageIndex]);
            PreviewSize = Helper.GetScaledSize(PreviewImage, DefaultPreviewSize);
        }
    }

    #endregion

    #region CTOR

    public PreviewImageViewModel()
    {
        if (!Design.IsDesignMode) return;
        StartPreview("C:/oxford-iiit-pet/images/Abyssinian/Abyssinian_1.jpg");
    }

    #endregion

    #region Public Properties

    public Bitmap? PreviewImage
    {
        get => _previewImage;
        set => this.RaiseAndSetIfChanged(ref _previewImage, value);
    }

    public Point PreviewPosition
    {
        get => _previewPosition;
        private set => this.RaiseAndSetIfChanged(ref _previewPosition, value);
    }

    public Size PreviewSize
    {
        get => _previewSize;
        private set => this.RaiseAndSetIfChanged(ref _previewSize, value);
    }

    public bool IsPreviewImageVisible
    {
        get => _isPreviewVisible;
        set => this.RaiseAndSetIfChanged(ref _isPreviewVisible, value);
    }

    public string PreviewCounter
    {
        get => _previewImageCounter;
        set => this.RaiseAndSetIfChanged(ref _previewImageCounter, value);
    }

    #endregion

    #region Public Methods

    public void StartPreview(string path)
    {
        _previewImagePaths = Helper.GetAllImagesInPath(path, Settings.Instance.PreviewDepth);
        if (_previewImagePaths is {Length: 0})
        {
            const string message = "No images found for preview";
            SendMessageToStatusBar(InfoBarSeverity.Warning, message);
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
        PreviewPosition = new Point(point.X, point.Y);
    }

    public void ZoomPreview(double deltaY)
    {
        if (!IsPreviewImageVisible) return;
        if (PreviewImage is null) return;
        var newDelta = (int) deltaY * 20;
        var newWidth = newDelta < 0
            ? Math.Max(150, PreviewSize.Width + newDelta)
            : Math.Min(600, PreviewSize.Width + newDelta);

        PreviewSize = Helper.GetScaledSize(PreviewImage, (int) newWidth);
    }


    public void ClosePreview()
    {
        if (!IsPreviewImageVisible) return;
        IsPreviewImageVisible = false;
        PreviewImage = null;
        _previewImagePaths = Array.Empty<string>();
    }

    #endregion
}