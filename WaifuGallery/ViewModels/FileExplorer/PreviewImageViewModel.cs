using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using WaifuGallery.Commands;
using WaifuGallery.Helpers;

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
    private string _previewImageCounter = "0/0";

    public string PreviewCounter
    {
        get => _previewImageCounter;
        set => this.RaiseAndSetIfChanged(ref _previewImageCounter, value);
    }


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
        }
    }

    #endregion

    #region CTOR

    public PreviewImageViewModel()
    {
        if (Design.IsDesignMode)
        {
            var i = Helper.GetAllImagesInPath("C:/oxford-iiit-pet/images/Abyssinian/Abyssinian_1.jpg");
            StartPreview(i);
        }

        PreviewImageSize = new Size(300, 300);
        PreviewImagePosition = new Point(0, 0);
    }

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

    private void SendCommandMessageBus(ICommandMessage command) => MessageBus.Current.SendMessage(command);

    public void StartPreview(string[] imagesInPath)
    {
        _previewImagePaths = imagesInPath;
        if (_previewImagePaths is {Length: 0})
        {
            const string message = "No images found for preview";
            var command = new SendMessageToStatusBarCommand(InfoBarSeverity.Warning, message);
            SendCommandMessageBus(command);
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
        SendCommandMessageBus(new ToggleFileExplorerScrollBarCommand());
    }

    #endregion
}