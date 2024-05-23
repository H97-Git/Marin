using System;
using Avalonia.Media.Imaging;
using ReactiveUI;
using WaifuGallery.Helpers;
using WaifuGallery.Models;

namespace WaifuGallery.ViewModels;

public sealed class FileViewModel : ViewModelBase
{
    #region Private Members

    private Bitmap? _image;
    private bool _isImage;
    private int _imageHeight;
    private int _imageWidth;
    private readonly string _fileName = "";
    private readonly string _parentDirPath = "";

    #endregion

    #region Public Properties

    public Bitmap? Image
    {
        get => _image;
        set
        {
            this.RaiseAndSetIfChanged(ref _image, value);
            ResizeThumbnail();
        }
    }

    public bool IsImage
    {
        get => _isImage;
        private set => this.RaiseAndSetIfChanged(ref _isImage, value);
    }

    public int ImageHeight
    {
        get => _imageHeight;
        set => this.RaiseAndSetIfChanged(ref _imageHeight, value);
    }

    public int ImageWidth
    {
        get => _imageWidth;
        set => this.RaiseAndSetIfChanged(ref _imageWidth, value);
    }

    public string FullPath => ParentDirPath + "\\" + FileName;

    public string FileName
    {
        get => _fileName;
        private init => this.RaiseAndSetIfChanged(ref _fileName, value);
    }

    public string ParentDirPath
    {
        get => _parentDirPath;
        private init => this.RaiseAndSetIfChanged(ref _parentDirPath, value);
    }

    #endregion

    #region Public Events

    public event EventHandler<Command>? OnSendCommandToFileExplorer;

    public void SendCommandToFileExplorer(Command command)
    {
        OnSendCommandToFileExplorer?.Invoke(this, command);
    }

    #endregion

    #region CTOR

    public FileViewModel(string parentDirPath, string fileName, Bitmap? image = null)
    {
        ParentDirPath = parentDirPath;
        FileName = fileName;
        if (image is null) return;
        Image = image;
        IsImage = true;
    }

    #endregion

    #region Private Methods

    private void ResizeThumbnail()
    {
        if (Image == null) return;
        // Console.WriteLine($"ResizeThumbnail: {Image.Size.Width}x{Image.Size.Height}");
        var imageSize = Image.Size.Width < Image.Size.Height
            ?
            // Image is vertical
            ImagesHelper.GetScaledSizeByHeight(Image, 100)
            :
            // Image is horizontal
            ImagesHelper.GetScaledSizeByWidth(Image, 100);

        ImageWidth = (int) imageSize.Width;
        ImageHeight = (int) imageSize.Height;
    }

    #endregion
}