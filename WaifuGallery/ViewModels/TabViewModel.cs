using System;
using System.IO;
using Avalonia.Media.Imaging;
using ReactiveUI;
using WaifuGallery.Helpers;

namespace WaifuGallery.ViewModels;

public class TabViewModel : ViewModelBase
{
    public string Header { get; set; }
}

public class ImageTabViewModel : TabViewModel
{
    #region Private Members

    private Bitmap? _currentBitmapImage;
    private bool _isCloseButtonVisible;
    private int _currentImageIndex;
    private int _currentImageHeight;
    private int _currentImageWidth;
    private string _currentDir;
    private string _currentImagePath;
    private string _tabHeaderContent;
    private string[] _imagesInPath;

    #endregion

    #region Public Properties

    public Bitmap? CurrentBitmapImage
    {
        get => _currentBitmapImage;
        set => this.RaiseAndSetIfChanged(ref _currentBitmapImage, value);
    }

    public bool IsCloseButtonVisible
    {
        get => _isCloseButtonVisible;
        set => this.RaiseAndSetIfChanged(ref _isCloseButtonVisible, value);
    }

    public int CurrentImageIndex
    {
        get => _currentImageIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentImageIndex, value);
            ChangeImage();
        }
    }

    public int CurrentImageHeight
    {
        get => _currentImageHeight;
        set => this.RaiseAndSetIfChanged(ref _currentImageHeight, value);
    }

    public int CurrentImageWidth
    {
        get => _currentImageWidth;
        set => this.RaiseAndSetIfChanged(ref _currentImageWidth, value);
    }

    public string CurrentDir
    {
        get => _currentDir;
        set => this.RaiseAndSetIfChanged(ref _currentDir, value);
    }

    public string CurrentImagePath
    {
        get => _currentImagePath;
        set => this.RaiseAndSetIfChanged(ref _currentImagePath, value);
    }

    public string[] ImagesInPath
    {
        get => _imagesInPath;
        set => this.RaiseAndSetIfChanged(ref _imagesInPath, value);
    }

    public string TabHeaderContent
    {
        get => _tabHeaderContent;
        set => this.RaiseAndSetIfChanged(ref _tabHeaderContent, value);
    }

    #endregion

    #region CTOR

    public ImageTabViewModel(string currentDir, string[] imagesInPath, int currentImageIndex)
    {
        CurrentDir = currentDir;
        ImagesInPath = imagesInPath;
        CurrentImageIndex = currentImageIndex;
    }

    #endregion

    #region Private Methods

    private void ChangeImage()
    {
        CurrentImagePath = ImagesInPath[CurrentImageIndex];
        CurrentBitmapImage = new Bitmap(CurrentImagePath);
        TabHeaderContent = SetTabHeaderContent();
    }

    private string SetTabHeaderContent()
    {
        const int maxLength = 12;
        var index = CurrentDir.Length > maxLength ? maxLength : CurrentDir.Length;

        return CurrentDir[..index] + ": " +
               Path.GetFileNameWithoutExtension(CurrentImagePath);
    }

    #endregion

    #region Public Methods

    public void LoadPreviousImage()
    {
        CurrentImageIndex = Math.Max(0, CurrentImageIndex - 1);
    }

    public void LoadNextImage()
    {
        CurrentImageIndex = Math.Min(ImagesInPath.Length - 1, CurrentImageIndex + 1);
    }

    public void LoadFirstImage()
    {
        CurrentImageIndex = 0;
    }

    public void LoadLastImage()
    {
        CurrentImageIndex = ImagesInPath.Length - 1;
    }

    public void ResizeImageByHeight(double targetHeight)
    {
        if (_currentBitmapImage == null) return;
        var imageSize = ImagesHelper.GetScaledSizeByHeight(_currentBitmapImage, (int) targetHeight);
        CurrentImageWidth = (int) imageSize.Width;
        CurrentImageHeight = (int) imageSize.Height;
    }

    public void ResizeImageByWidth(double targetHeight)
    {
        if (_currentBitmapImage == null) return;
        var imageSize = ImagesHelper.GetScaledSizeByWidth(_currentBitmapImage, (int) targetHeight);
        CurrentImageWidth = (int) imageSize.Width;
        CurrentImageHeight = (int) imageSize.Height;
    }

    #endregion

    public void ZoomImage(double deltaY)
    {
        const int offset = 100;
        if (_currentBitmapImage == null) return;
        if (deltaY > 0)
        {
            CurrentImageHeight += (int) deltaY + offset;
            CurrentImageWidth += (int) deltaY + offset;
        }
        else
        {
            CurrentImageHeight -= (int) deltaY + offset;
            CurrentImageWidth -= (int) deltaY + offset;
        }
    }
}