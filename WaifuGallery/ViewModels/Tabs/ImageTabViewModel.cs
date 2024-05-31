using System;
using System.IO;
using Avalonia;
using Avalonia.Media.Imaging;
using ReactiveUI;
using WaifuGallery.Helpers;

namespace WaifuGallery.ViewModels.Tabs;

public class ImageTabViewModel : TabViewModelBase
{
    #region Private Members

    private Bitmap _bitmapImage;
    private int _index;
    private Size _imageSize;
    private string _parentFolderName;
    private string _imagePath;
    private string _tabHeaderContent;
    private string[] _imagesInPath;

    #endregion

    #region Public Properties

    public Bitmap BitmapImage
    {
        get => _bitmapImage;
        set => this.RaiseAndSetIfChanged(ref _bitmapImage, value);
    }

    public Size ImageSize
    {
        get => _imageSize;
        set => this.RaiseAndSetIfChanged(ref _imageSize, value);
    }

    public int Index
    {
        get => _index;
        set
        {
            if (value < 0)
                value = 0;
            if (value >= ImagesInPath.Length)
                value = ImagesInPath.Length - 1;
            this.RaiseAndSetIfChanged(ref _index, value);
            ChangeImage();
        }
    }

    public string ParentFolderName
    {
        get => _parentFolderName;
        set => this.RaiseAndSetIfChanged(ref _parentFolderName, value);
    }

    public string ImagePath
    {
        get => _imagePath;
        set => this.RaiseAndSetIfChanged(ref _imagePath, value);
    }

    public string[] ImagesInPath
    {
        get => _imagesInPath;
        set => this.RaiseAndSetIfChanged(ref _imagesInPath, value);
    }

    #endregion

    #region CTOR

    public ImageTabViewModel(string parentFolderName, string[] imagesInPath, int index)
    {
        ParentFolderName = parentFolderName;
        ImagesInPath = imagesInPath;
        Index = index;
    }

    #endregion

    #region Private Methods

    private void ChangeImage()
    {
        ImagePath = ImagesInPath[Index];
        BitmapImage = new Bitmap(ImagePath);
        Header = SetTabHeaderContent();
    }

    private string SetTabHeaderContent()
    {
        const int maxLength = 12;
        var index = ParentFolderName.Length > maxLength ? maxLength : ParentFolderName.Length;

        return ParentFolderName[..index] + ": " +
               Path.GetFileNameWithoutExtension(ImagePath);
    }

    #endregion

    #region Public Methods

    public void LoadPreviousImage()
    {
        Index -= 1;
    }

    public void LoadNextImage()
    {
        Index += 1;
    }

    public void LoadFirstImage()
    {
        Index = 0;
    }

    public void LoadLastImage()
    {
        Index = ImagesInPath.Length - 1;
    }

    public void ResizeImageByHeight(double targetHeight) =>
        ImageSize = ImagesHelper.GetScaledSizeByHeight(_bitmapImage, (int) targetHeight);

    public void ResizeImageByWidth(double targetHeight) =>
        ImageSize = ImagesHelper.GetScaledSizeByWidth(_bitmapImage, (int) targetHeight);

    public void ZoomImage(double deltaY)
    {
        var newDelta = (int) deltaY * 100;
        double newWidth;
        double newHeight;
        if (newDelta < 0)
        {
            newWidth = Math.Max(200, ImageSize.Width + newDelta);
            newHeight = Math.Max(200, ImageSize.Height + newDelta);
        }
        else
        {
            newWidth = ImageSize.Width + newDelta;
            newHeight = ImageSize.Height + newDelta;
        }

        ImageSize = new Size(newWidth, newHeight);
    }

    #endregion
}