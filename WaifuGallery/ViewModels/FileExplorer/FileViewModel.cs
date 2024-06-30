using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Media.Imaging;
using FluentAvalonia.UI.Controls;
using ImageMagick;
using ReactiveUI;
using WaifuGallery.Commands;
using WaifuGallery.Helpers;

namespace WaifuGallery.ViewModels.FileExplorer;

public sealed class FileViewModel : ViewModelBase
{
    #region Private Fields

    private Bitmap _thumbnail;
    private Size _imageSize;
    private Symbol _symbol = Symbol.Folder;
    private bool _isDirectoryEmpty;
    private bool _isFileReadOnly;
    private bool _isRenaming;
    private bool _isImage;
    private long _sizeInBytes;
    private string _fileName = string.Empty;
    private string _createdTime = string.Empty;
    private string _lastAccessTime = string.Empty;

    #endregion

    #region Private Properties

    private static string ThumbnailsPath => Path.Combine(Settings.SettingsPath, "Thumbnails");

    private long SizeInBytes
    {
        get => _sizeInBytes;
        set => this.RaiseAndSetIfChanged(ref _sizeInBytes, value);
    }

    private bool IsFileReadOnly
    {
        get => _isFileReadOnly;
        set => this.RaiseAndSetIfChanged(ref _isFileReadOnly, value);
    }

    #endregion

    #region Private Methods

    private async void Initialize(FileSystemInfo fileSystemInfo)
    {
        Directory.CreateDirectory(ThumbnailsPath);
        switch (fileSystemInfo)
        {
            case DirectoryInfo directoryInfo:
                ParentPath = directoryInfo.Parent?.FullName;
                SizeInBytes = Helper.GetDirectorySizeInByte(fileSystemInfo);
                _isDirectoryEmpty = Helper.IsDirectoryEmpty(fileSystemInfo);
                Symbol = _isDirectoryEmpty ? Symbol.Folder : Symbol.FolderFilled;
                break;
            case FileInfo fileInfo:
                ParentPath = fileInfo.DirectoryName;
                SizeInBytes = fileInfo.Length;
                IsFileReadOnly = fileInfo.IsReadOnly;
                switch (fileInfo.Extension)
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                    case ".bmp":
                    case ".gif":
                        IsImage = true;
                        Thumbnail = await GetThumbnail(new FileInfo(fileSystemInfo.FullName));
                        break;
                    case ".mp4":
                    case ".avi":
                    case ".mkv":
                    case ".mov":
                    case ".wmv":
                    case ".webm":
                    case ".flv":
                        Symbol = Symbol.Video;
                        break;
                    case ".zip":
                    case ".rar":
                    case ".7z":
                        Symbol = Symbol.ZipFolder;
                        break;
                }

                break;
        }

        FullPath = fileSystemInfo.FullName;
        FileName = fileSystemInfo.Name;
        CreatedTime = fileSystemInfo.CreationTime.ToString(CultureInfo.InvariantCulture);
        LastAccessTime = fileSystemInfo.LastAccessTime.ToString(CultureInfo.InvariantCulture);
    }

    private void ResizeThumbnail()
    {
        var isPortrait = Thumbnail.Size.Width < Thumbnail.Size.Height;
        ImageSize = isPortrait
            ? Helper.GetScaledSizeByHeight(Thumbnail, 100)
            : Helper.GetScaledSizeByWidth(Thumbnail, 100);
    }

    #endregion

    #region CTOR

    public FileViewModel()
    {
        var directoryInfo = new DirectoryInfo(@"C:\oxford-iiit-pet\images");
        Initialize(directoryInfo);
    }

    public FileViewModel(FileSystemInfo fileSystemInfo)
    {
        Initialize(fileSystemInfo);
    }

    #endregion

    private static async Task<Bitmap> GenerateBitmapThumb(FileInfo sourceFileInfo, FileInfo outputFileInfo)
    {
        using var image = new MagickImage(sourceFileInfo);
        image.Resize(new MagickGeometry(100, 100)
        {
            IgnoreAspectRatio = false
        });
        await image.WriteAsync(outputFileInfo);
        return new Bitmap(outputFileInfo.FullName);
    }

   private async Task<Bitmap?> GetThumbnail(FileInfo fileInfo)
    {
        if (fileInfo.Directory?.Name == null) return null;
        var dirInCacheForCurrentFile = Path.Combine(ThumbnailsPath, fileInfo.Directory.Name);
        Directory.CreateDirectory(dirInCacheForCurrentFile);
        var cachedImagesPath = Helper.GetAllImagesInPath(dirInCacheForCurrentFile);
        if (cachedImagesPath is {Length: 0})
        {
            var outputFileInfo = new FileInfo(Path.Combine(dirInCacheForCurrentFile, fileInfo.Name));
            return await GenerateBitmapThumb(fileInfo, outputFileInfo);
        }

        var thumbnailPath = cachedImagesPath.FirstOrDefault(x => Path.GetFileName(x) == fileInfo.Name);
        if (!File.Exists(thumbnailPath))
        {
            var outputFileInfo = new FileInfo(Path.Combine(dirInCacheForCurrentFile, fileInfo.Name));
            return await GenerateBitmapThumb(fileInfo, outputFileInfo);
        }

        return new Bitmap(thumbnailPath);
    }

    #region Public Properties

    public Bitmap Thumbnail
    {
        get => _thumbnail;
        set
        {
            this.RaiseAndSetIfChanged(ref _thumbnail, value);
            ResizeThumbnail();
        }
    }

    public Symbol Symbol
    {
        get => _symbol;
        set => this.RaiseAndSetIfChanged(ref _symbol, value);
    }

    public bool IsImage
    {
        get => _isImage;
        private set => this.RaiseAndSetIfChanged(ref _isImage, value);
    }

    public bool IsRenaming
    {
        get => _isRenaming;
        set => this.RaiseAndSetIfChanged(ref _isRenaming, value);
    }

    public Size ImageSize
    {
        get => _imageSize;
        private set => this.RaiseAndSetIfChanged(ref _imageSize, value);
    }

    public string FileName
    {
        get => _fileName;
        set => this.RaiseAndSetIfChanged(ref _fileName, value);
    }

    public string LastAccessTime
    {
        get => _lastAccessTime;
        set => this.RaiseAndSetIfChanged(ref _lastAccessTime, value);
    }

    public string CreatedTime
    {
        get => _createdTime;
        set => this.RaiseAndSetIfChanged(ref _createdTime, value);
    }

    public string ReadOnly => $"Read Only: {IsFileReadOnly}";

    public string SizeInHumanReadable
    {
        get
        {
            return SizeInBytes switch
            {
                < 1024 => $"{SizeInBytes} B",
                < 1024 * 1024 => $"{SizeInBytes / 1024} KB",
                < 1024 * 1024 * 1024 => $"{SizeInBytes / 1024 / 1024} MB",
                _ => $"{SizeInBytes / 1024 / 1024 / 1024} GB"
            };
        }
    }

    public string FullPath { get; private set; } = string.Empty;
    public string? ParentPath { get; private set; }

    #endregion

    #region Public Commands

    public ICommand Copy =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new CopyCommand(FullPath)); });

    public ICommand Cut =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new CutCommand(FullPath)); });

    public ICommand Delete =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new DeleteCommand(FullPath)); });

    public ICommand Rename =>
        ReactiveCommand.Create(() => { IsRenaming = true; });

    public ICommand Paste =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new PasteCommand(FullPath)); });

    #endregion
}