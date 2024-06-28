using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Avalonia;
using Avalonia.Media.Imaging;
using ReactiveUI;
using WaifuGallery.Commands;
using WaifuGallery.Helpers;

namespace WaifuGallery.ViewModels.Tabs;

public class ImageTabViewModel : TabViewModelBase
{
    #region Private Fields

    private Bitmap? _bitmapImage;
    private Size _imageSize;
    private readonly string[] _imagesInPath;
    private readonly string? _parentFolderName;
    private int _index;

    #endregion

    #region Private Properties

    private int Index
    {
        get => _index;
        set
        {
            if (value < 0)
                value = 0;
            else if (value >= _imagesInPath.Length)
                value = _imagesInPath.Length - 1;
            _index = value;
            LoadImage();
        }
    }

    private string CurrentImagePath => _imagesInPath[Index];

    #endregion

    #region Private Methods

    private void LoadImage()
    {
        BitmapImage = new Bitmap(CurrentImagePath);
        SetTabHeaderContent();
        MessageBus.Current.SendMessage(new FitToHeightCommand());
    }

    private void SetTabHeaderContent()
    {
        const int maxLength = 12;
        if (_parentFolderName is null)
        {
            Path.GetFileNameWithoutExtension(CurrentImagePath);
            return;
        }

        var index = _parentFolderName is {Length: > maxLength} ? maxLength : _parentFolderName.Length;
        Header = _parentFolderName[..index] + ": " + Path.GetFileNameWithoutExtension(CurrentImagePath);
    }

    private static string GenerateUniqueId(string path)
    {
        var salt = new Random().Next();
        var data = Encoding.UTF8.GetBytes(path + salt);
        var hashBytes = SHA256.HashData(data);
        var hashStringBuilder = new StringBuilder(64);
        foreach (var b in hashBytes)
        {
            hashStringBuilder.Append(b.ToString("x2"));
        }

        return hashStringBuilder.ToString();
    }

    #endregion

    #region CTOR

    private ImageTabViewModel(string id, string[] imagesInPath, int index)
    {
        Id = id;
        _imagesInPath = imagesInPath;
        _parentFolderName = Directory.GetParent(_imagesInPath.First())?.Name;
        Index = index;
    }

    #endregion

    #region Public Properties

    public Bitmap? BitmapImage
    {
        get => _bitmapImage;
        set => this.RaiseAndSetIfChanged(ref _bitmapImage, value);
    }

    public Size ImageSize
    {
        get => _imageSize;
        private set => this.RaiseAndSetIfChanged(ref _imageSize, value);
    }

    public Matrix Matrix { get; set; }
    public bool IsDefaultZoom { get; set; } = true;

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
        ImageSize = Helper.GetScaledSizeByHeight(BitmapImage, (int) targetHeight);

    public void ResizeImageByWidth(double targetHeight) =>
        ImageSize = Helper.GetScaledSizeByWidth(BitmapImage, (int) targetHeight);

    public static ImageTabViewModel? CreateImageTabFromCommand(ICommandMessage command)
    {
        switch (command)
        {
            case OpenInNewTabCommand openInNewTabCommand:
            {
                var id = GenerateUniqueId(openInNewTabCommand.ImagesInPath.First());
                var imagesInPath = openInNewTabCommand.ImagesInPath;
                return new ImageTabViewModel(id, imagesInPath, openInNewTabCommand.Index);
            }
            case OpenFileCommand openFileCommand:
            {
                var path = openFileCommand.Path;
                if (path == null) return null;
                var imagesInPath = Helper.GetAllImagesInPath(path);
                var index = Array.IndexOf(imagesInPath, path);
                var id = GenerateUniqueId(imagesInPath.First());
                return new ImageTabViewModel(id, imagesInPath, index);
            }
        }

        return null;
    }

    #endregion
}