using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using ReactiveUI;
using WaifuGallery.Helpers;

namespace WaifuGallery.ViewModels.Tabs;

public class ImageTabViewModel : TabViewModelBase
{
    #region Private Members

    private Bitmap _bitmapImage;
    private Size _imageSize;
    private Point _imagePosition;
    private readonly string[] _imagesInPath;
    private readonly string? _parentFolderName;
    private int _index;

    private int Index
    {
        get => _index;
        set
        {
            if (value < 0)
                value = 0;
            if (value >= _imagesInPath.Length)
                value = _imagesInPath.Length - 1;
            _index = value;
            BitmapImage = new Bitmap(CurrentImagePath);
        }
    }

    private string CurrentImagePath => _imagesInPath[Index];

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
        private set => this.RaiseAndSetIfChanged(ref _imageSize, value);
    }

    public Point ImagePosition
    {
        get => _imagePosition;
        set
        {
            Console.WriteLine(value: $"Before value: {value}");
            if (value.Y < 0)
                value = new Point(value.X, 0);
            if (value.X < 0)
                value = new Point(0, value.Y);
            Console.WriteLine(value: $"After value: {value}");
            this.RaiseAndSetIfChanged(ref _imagePosition, value);
        }
    }

    #endregion

    #region CTOR

    public ImageTabViewModel(Guid id, string[] imagesInPath, int index)
    {
        Id = id;
        _imagesInPath = imagesInPath;
        _parentFolderName = Directory.GetParent(_imagesInPath.First())?.Name;
        Index = index;
        Header = SetTabHeaderContent();
        BitmapImage = new Bitmap(CurrentImagePath);
    }

    #endregion

    #region Private Methods

    private string SetTabHeaderContent()
    {
        const int maxLength = 12;
        if (_parentFolderName is null) return Path.GetFileNameWithoutExtension(CurrentImagePath);
        var index = _parentFolderName is {Length: > maxLength} ? maxLength : _parentFolderName.Length;
        return _parentFolderName[..index] + ": " +
               Path.GetFileNameWithoutExtension(CurrentImagePath);
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
        Index = _imagesInPath.Length - 1;
    }

    public void ResizeImageByHeight(double targetHeight) =>
        ImageSize = Helper.GetScaledSizeByHeight(_bitmapImage, (int) targetHeight);

    public void ResizeImageByWidth(double targetHeight) =>
        ImageSize = Helper.GetScaledSizeByWidth(_bitmapImage, (int) targetHeight);

    public void ZoomImage(double deltaY)
    {
        const double zoomFactor = 1.1;
        if (deltaY < 0)
        {
            var newWidth = Math.Max(300, ImageSize.Width / zoomFactor);
            var newHeight = Math.Max(300, ImageSize.Height / zoomFactor);

            ImageSize = new Size(newWidth, newHeight);
        }
        else
        {
            ImageSize *= zoomFactor;
            // newWidth = ImageSize.Width + newDelta;
            // newHeight = ImageSize.Height + newDelta;
        }

        // ImageSize = new Size(newWidth, newHeight);
    }

    #endregion
}