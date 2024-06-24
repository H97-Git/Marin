using System.Globalization;
using System.IO;
using System.Windows.Input;
using Avalonia;
using Avalonia.Media.Imaging;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using WaifuGallery.Commands;
using WaifuGallery.Helpers;

namespace WaifuGallery.ViewModels.FileExplorer;

public sealed class FileViewModel : ViewModelBase
{
    #region Private Members

    private Bitmap? _thumbnail;
    private Size _imageSize;
    private Symbol _symbol = Symbol.Folder;
    private bool _isFileReadOnly;
    private bool _isImage;
    private bool _isDirectoryEmpty;
    private long _sizeInBytes;
    private readonly FileSystemInfo _fileSystemInfo;
    private readonly string _fileName = string.Empty;
    private string _createdTime = string.Empty;
    private string _lastAccessTime = string.Empty;

    #endregion

    #region Public Properties

    public Bitmap? Thumbnail
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

    private long SizeInBytes
    {
        get => _sizeInBytes;
        init => this.RaiseAndSetIfChanged(ref _sizeInBytes, value);
    }

    public bool IsFileReadOnly
    {
        get => _isFileReadOnly;
        set => this.RaiseAndSetIfChanged(ref _isFileReadOnly, value);
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

    public string FullPath => _fileSystemInfo.FullName;
    public string? ParentPath { get; }

    #endregion

    #region CTOR

    public FileViewModel(FileSystemInfo fileSystemInfo, Bitmap? thumbnail = null)
    {
        _fileSystemInfo = fileSystemInfo;
        switch (_fileSystemInfo)
        {
            case DirectoryInfo directoryInfo:
                ParentPath = directoryInfo.Parent?.FullName;
                SizeInBytes = Helper.GetDirectorySizeInByte(_fileSystemInfo);
                _isDirectoryEmpty = Helper.IsDirectoryEmpty(_fileSystemInfo);
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

        FileName = _fileSystemInfo.Name;
        CreatedTime = _fileSystemInfo.CreationTime.ToString(CultureInfo.InvariantCulture);
        LastAccessTime = _fileSystemInfo.LastAccessTime.ToString(CultureInfo.InvariantCulture);
        if (thumbnail is null) return;
        Thumbnail = thumbnail;
    }

    #endregion

    #region Private Methods

    private void ResizeThumbnail()
    {
        if (Thumbnail is null) return;
        var isPortrait = Thumbnail.Size.Width < Thumbnail.Size.Height;
        ImageSize = isPortrait
            ? Helper.GetScaledSizeByHeight(Thumbnail, 100)
            : Helper.GetScaledSizeByWidth(Thumbnail, 100);
    }

    #endregion

    #region Public Commands

    public ICommand Copy =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new CopyCommand(FullPath)); });

    public ICommand Cut =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new CutCommand(FullPath)); });

    public ICommand Delete =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new DeleteCommand()); });

    public ICommand Move =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new MoveCommand()); });

    public ICommand Paste =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new PasteCommand()); });

    #endregion
}