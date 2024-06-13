using System;
using System.IO;
using Avalonia;
using Avalonia.Media.Imaging;
using ReactiveUI;
using WaifuGallery.Helpers;
using WaifuGallery.Models;

namespace WaifuGallery.ViewModels.FileExplorer;

public sealed class FileViewModel : ViewModelBase
{
    #region Private Members

    private Bitmap? _image;
    private Size _imageSize;
    private bool _isImage;
    private readonly FileSystemInfo _fileSystemInfo;
    private readonly string _fileName = string.Empty;

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

    public Size ImageSize
    {
        get => _imageSize;
        set => this.RaiseAndSetIfChanged(ref _imageSize, value);
    }

    public string FileName
    {
        get => _fileName;
        private init => this.RaiseAndSetIfChanged(ref _fileName, value);
    }
    
    public string FullPath => _fileSystemInfo.FullName;
    public string? ParentPath { get; }

    #endregion

    #region CTOR

    public FileViewModel(FileSystemInfo fileSystemInfo, Bitmap? image = null)
    {
        _fileSystemInfo = fileSystemInfo;
        ParentPath = _fileSystemInfo switch
        {
            DirectoryInfo directoryInfo => directoryInfo.Parent?.FullName,
            FileInfo fileInfo => fileInfo.DirectoryName,
            _ => ParentPath
        };

        FileName = fileSystemInfo.Name;
        if (image is null) return;
        Image = image;
        IsImage = true;
    }

    #endregion

    #region Public Events

    public event EventHandler<Command>? OnSendCommandToFileExplorer;

    #endregion

    #region Private Methods

    private void ResizeThumbnail()
    {
        if (Image is null) return;
        var isPortrait = Image.Size.Width < Image.Size.Height;
        ImageSize = isPortrait
            ? ImagesHelper.GetScaledSizeByHeight(Image, 100)
            : ImagesHelper.GetScaledSizeByWidth(Image, 100);
    }

    #endregion

    #region Public Methods

    public void SendCommandToFileExplorer(Command command)
    {
        OnSendCommandToFileExplorer?.Invoke(this, command);
    }

    #endregion
}