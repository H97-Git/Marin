using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Media.Imaging;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using WaifuGallery.Commands;
using WaifuGallery.Helpers;
using WaifuGallery.Models;

namespace WaifuGallery.ViewModels.FileManager;

public sealed class FileViewModel : ViewModelBase
{
    #region Private Fields

    private readonly FileSystemInfo _fileSystemInfo;
    private Bitmap? _thumbnail;
    private Size _imageSize;
    private Symbol _symbol = Symbol.Folder;
    private bool _isArchive;
    private bool _isDirectoryEmpty;
    private bool _isFileReadOnly;
    private bool _isImage;
    private bool _isDirectory;
    private bool _isRenaming;
    private long _sizeInBytes;
    private string _createdTime = string.Empty;
    private string _fileName = string.Empty;
    private string _lastAccessTime = string.Empty;

    #endregion

    #region Private Properties

    private static string ThumbnailsPath => Settings.ThumbnailsPath;

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

    private void Initialize()
    {
        Directory.CreateDirectory(ThumbnailsPath);
        switch (_fileSystemInfo)
        {
            case DirectoryInfo directoryInfo:
                ParentPath = directoryInfo.Parent?.FullName;
                if (Settings.Instance.ShouldCalculateFolderSize)
                    SizeInBytes = Helper.GetDirectorySizeInByte(_fileSystemInfo);
                _isDirectoryEmpty = Helper.IsDirectoryEmpty(_fileSystemInfo);
                Symbol = _isDirectoryEmpty ? Symbol.Folder : Symbol.FolderFilled;
                IsDirectory = true;
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
                        Symbol = Symbol.Image;
                        IsImage = true;
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
                        IsArchive = true;
                        break;
                }

                break;
        }

        FullPath = _fileSystemInfo.FullName;
        FileName = _fileSystemInfo.Name;
        CreatedTime = _fileSystemInfo.CreationTime.ToString(CultureInfo.InvariantCulture);
        LastAccessTime = _fileSystemInfo.LastAccessTime.ToString(CultureInfo.InvariantCulture);

        this.WhenAnyValue(x => x.Thumbnail)
            .Subscribe(_ =>
            {
                if (Thumbnail is null) return;
                ImageSize = Helper.GetScaledSize(Thumbnail, 100);
            });
    }

    #endregion

    #region CTOR

    public FileViewModel()
    {
        var directoryInfo = new DirectoryInfo(@"C:\oxford-iiit-pet\images");
        _fileSystemInfo = directoryInfo;
        Initialize();
    }

    public FileViewModel(FileSystemInfo fileSystemInfo)
    {
        _fileSystemInfo = fileSystemInfo;
        Initialize();
    }

    #endregion

    #region Public Properties

    public Bitmap? Thumbnail
    {
        get => _thumbnail;
        set => this.RaiseAndSetIfChanged(ref _thumbnail, value);
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

    public bool IsArchive
    {
        get => _isArchive;
        set => this.RaiseAndSetIfChanged(ref _isArchive, value);
    }

    public bool IsDirectory
    {
        get => _isDirectory;
        set => this.RaiseAndSetIfChanged(ref _isDirectory, value);
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

    #region Public Methods

    public async Task GetThumbnail()
    {
        if (_fileSystemInfo is not FileInfo fileInfo)
        {
            throw new InvalidOperationException();
        }

        if (Helper.ThumbnailExists(fileInfo, out var thumbnailPath))
        {
            Thumbnail = new Bitmap(thumbnailPath);
            return;
        }

        var outputFileInfo = new FileInfo(Path.Combine(thumbnailPath, fileInfo.Name));
        Thumbnail = await Helper.GenerateBitmapThumb(fileInfo, outputFileInfo);
    }

    #endregion

    #region Public Commands

    public ICommand Copy =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new CopyCommand(FullPath)); });

    public ICommand Cut =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new CutCommand(FullPath)); });

    public ICommand Delete =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new DeleteCommand(FullPath)); });

    public ICommand Extract =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new ExtractCommand(FullPath)); });

    public ICommand CalculateSize =>
        ReactiveCommand.Create(() =>
        {
            if (SizeInBytes is 0)
            {
                SizeInBytes = Helper.GetDirectorySizeInByte(_fileSystemInfo);
            }
        });

    public ICommand Rename =>
        ReactiveCommand.Create(() => { IsRenaming = true; });

    public ICommand Paste =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new PasteCommand(FullPath)); });

    public ICommand OpenInExplorer =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new OpenInFileExplorerCommand(FullPath)); });

    public ICommand OpenInBrowser =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new OpenInBrowserCommand(FullPath)); });

    #endregion
}