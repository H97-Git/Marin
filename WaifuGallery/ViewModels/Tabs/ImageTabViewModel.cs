using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DynamicData;
using ReactiveUI;
using Serilog;
using WaifuGallery.Commands;
using WaifuGallery.Helpers;
using WaifuGallery.Models;

namespace WaifuGallery.ViewModels.Tabs;

public class ImageTabViewModel : TabViewModelBase
{
    #region Private Fields

    private Bitmap? _bitmapImage;
    private Size _imageSize;
    private readonly string[] _imagesInPath;
    private readonly string? _parentFolderName;
    private int _index;
    private int _rotationAngle;
    private bool _isGridOpen;

    #endregion

    #region Private Properties

    private int Index
    {
        get => _index;
        set
        {
            _index = SetIndexInBound(value);
            LoadImage();
        }
    }

    private int SetIndexInBound(int value)
    {
        if (value < 0)
        {
            if (Settings.Instance.TabsPreference.Loop)
            {
                return _imagesInPath.Length - 1;
            }

            return 0;
        }

        if (value >= _imagesInPath.Length)
        {
            if (Settings.Instance.TabsPreference.Loop)
            {
                return 0;
            }

            return _imagesInPath.Length - 1;
        }

        return value;
    }

    public string CurrentImagePath => _imagesInPath[Index];

    #endregion

    #region Private Methods

    private void LoadImage()
    {
        Log.Debug("LoadImage");
        BitmapImage = new Bitmap(CurrentImagePath);
        SetTabHeaderContent();
        var clientSize = App.GetClientSize();
        var imageAspectRatio = BitmapImage.Size.Width / BitmapImage.Size.Height;
        var clientAspectRatio = clientSize?.Width / clientSize?.Height ?? 1;

        if (clientSize is not null)
        {
            if (imageAspectRatio > clientAspectRatio)
            {
                // Image is in portrait.
                MessageBus.Current.SendMessage(new FitToWidthCommand());
            }
            else
            {
                // Image is in landscape.
                MessageBus.Current.SendMessage(new FitToHeightCommand());
            }
        }
        else
        {
            // Default.
            MessageBus.Current.SendMessage(new FitToHeightCommand());
        }
    }

    private void SetTabHeaderContent()
    {
        Log.Debug("SetTabHeaderContent");
        const int maxLength = 12;
        if (_parentFolderName is null)
        {
            Path.GetFileNameWithoutExtension(CurrentImagePath);
            return;
        }

        var index = _parentFolderName is {Length: > maxLength} ? maxLength : _parentFolderName.Length;
        Header = _parentFolderName[..index] + " > " + Path.GetFileNameWithoutExtension(CurrentImagePath);
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

        var hash = hashStringBuilder.ToString();
        Log.Debug("GenerateUniqueId path: {Path}, hash: {Hash}", path, hash);
        return hash;
    }

    private async void LoadBitmaps()
    {
        Log.Debug("Loading Bitmaps for grid mode(?)");
        const int batchSize = 10;
        var bufferList = new List<Bitmap>(); // So we can use AddRange.
        if (Bitmaps.Count is not 0) return;
        for (var index = 0; index < _imagesInPath.Length; index += batchSize)
        {
            var batch = _imagesInPath.Skip(index).Take(batchSize).ToArray();
            var batchList = batch.Select(path => new FileInfo(path));
            foreach (var fileInfo in batchList)
            {
                if (ThumbnailHelper.Exists(fileInfo, out var thumbnailPath))
                {
                    bufferList.Add(new Bitmap(thumbnailPath));
                }
                else
                {
                    var outputPath = Path.Combine(thumbnailPath, fileInfo.Name);
                    var bitmap = await Task.Run(() =>
                        ThumbnailHelper.GenerateAsync(fileInfo, new FileInfo(outputPath)));
                    bufferList.Add(bitmap);
                }
            }

            await Dispatcher.UIThread.InvokeAsync(() => Bitmaps.AddRange(bufferList));
            await Task.Delay(100, CancellationToken.None);
            bufferList.Clear();
        }
    }

    #endregion

    #region CTOR

    private ImageTabViewModel(string id, string[] imagesInPath, int index)
    {
        Id = id;
        _imagesInPath = imagesInPath;
        _parentFolderName = Directory.GetParent(_imagesInPath.First())?.Name;
        Index = index;
        LoadBitmaps();
    }

    #endregion

    #region Public Properties

    public ObservableCollection<Bitmap> Bitmaps { get; set; } = [];

    public bool IsGridOpen
    {
        get => _isGridOpen;
        set => this.RaiseAndSetIfChanged(ref _isGridOpen, value);
    }

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

    public int RotationAngle
    {
        get => _rotationAngle;
        set => this.RaiseAndSetIfChanged(ref _rotationAngle, value);
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

    public void ResizeImageByHeight(double targetHeight)
    {
        Log.Debug("ResizeImageByHeight");
        if (BitmapImage != null) ImageSize = ImageSizeHelper.GetScaledSizeByHeight(BitmapImage, (int) targetHeight);
    }

    public void ResizeImageByWidth(double targetHeight)
    {
        Log.Debug("ResizeImageByWidth");
        if (BitmapImage != null) ImageSize = ImageSizeHelper.GetScaledSizeByWidth(BitmapImage, (int) targetHeight);
    }

    public void RotateImage(bool clockwise)
    {
        Log.Debug("RotateImage");
        if (clockwise)
        {
            RotationAngle = (RotationAngle + 90) % 360;
        }
        else
        {
            RotationAngle = (RotationAngle - 90) % 360;
            if (RotationAngle < 0)
                RotationAngle += 360;
        }
    }

    public void ToggleGrid()
    {
        Log.Debug("Open grid mode");
        IsGridOpen = !IsGridOpen;
    }

    public void CloseGrid()
    {
        Log.Debug("Close grid mode");
        IsGridOpen = false;
    }

    public void GridSelected(int? selectedIndex)
    {
        Log.Debug("GridSelected {Index}", selectedIndex);
        if (selectedIndex is null) return;
        Index = selectedIndex.Value;
        IsGridOpen = false;
    }

    public static ImageTabViewModel? CreateImageTabFromCommand(ICommandMessage command)
    {
        Log.Debug("Creating an image tab from ICommandMessage");
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
                var imagesInPath = PathHelper.GetAllImages(path);
                var index = Array.IndexOf(imagesInPath, path);
                var id = GenerateUniqueId(imagesInPath.First());
                return new ImageTabViewModel(id, imagesInPath, index);
            }
        }

        return null;
    }

    #endregion
}