using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ImageMagick;
using ReactiveUI;
using WaifuGallery.Helpers;
using WaifuGallery.Models;

namespace WaifuGallery.ViewModels.FileExplorer;

public class FileExplorerViewModel : ViewModelBase
{
    #region Private Members

    private ObservableCollection<FileViewModel> _filesInDir = [];
    private ScrollBarVisibility _scrollBarVisibility = ScrollBarVisibility.Auto;

    private Brush? _fileExplorerBackground;
    private FileViewModel[] _selectedItem;
    private bool _isFileExplorerExpanded = true;
    private bool _isFileExplorerVisible = true;
    private bool _isSearchFocused;
    private int _columnsCount;
    private int _imagesInPathCount = 0;
    private int _selectedIndexInFileExplorer;
    private bool _isPointerOver;
    private readonly List<string> _pathHistory = [];

    public FileViewModel[] SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    private static List<FilePickerFileType> GetFileTypes() => [FilePickerFileTypes.ImageAll];

    private static string SettingsPath
    {
        get
        {
            if (OperatingSystem.IsWindows())
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\WaifuGallery";
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.WaifuGallery";
        }
    }

    private static string ThumbnailsPath
    {
        get
        {
            if (OperatingSystem.IsWindows())
                return SettingsPath + "\\thumbnails";
            return SettingsPath + "/thumbnails";
        }
    }

    private string _currentPath = string.Empty;
    private string[]? _cachedImagesPath;

    #endregion

    #region Public Properties

    public bool IsPointerOver
    {
        get => _isPointerOver;
        set => this.RaiseAndSetIfChanged(ref _isPointerOver, value);
    }

    public Brush? FileExplorerBackground
    {
        get => _fileExplorerBackground;
        set => this.RaiseAndSetIfChanged(ref _fileExplorerBackground, value);
    }

    public PreviewImageViewModel PreviewImageViewModel { get; }

    public ObservableCollection<FileViewModel> FilesInDir
    {
        get => _filesInDir;
        set => this.RaiseAndSetIfChanged(ref _filesInDir, value);
    }

    public FileViewModel SelectedFile => FilesInDir[SelectedIndex];

    public ScrollBarVisibility ScrollBarVisibility
    {
        get => _scrollBarVisibility;
        set => this.RaiseAndSetIfChanged(ref _scrollBarVisibility, value);
    }

    public bool IsFileExplorerVisible
    {
        get => _isFileExplorerVisible;
        set => this.RaiseAndSetIfChanged(ref _isFileExplorerVisible, value);
    }

    public bool IsFileExplorerExpanded
    {
        get => _isFileExplorerExpanded;
        set
        {
            this.RaiseAndSetIfChanged(ref _isFileExplorerExpanded, value);
            FileExplorerBackground = IsFileExplorerExpanded ? new SolidColorBrush(Colors.Transparent) : null;
        }
    }

    public bool IsSearchFocused
    {
        get => _isSearchFocused;
        set => this.RaiseAndSetIfChanged(ref _isSearchFocused, value);
    }

    public int ColumnsCount
    {
        get => _columnsCount;
        set => this.RaiseAndSetIfChanged(ref _columnsCount, value);
    }

    public int SelectedIndex
    {
        get => _selectedIndexInFileExplorer;
        set
        {
            if (value < 0)
                value = 0;

            if (value >= FilesInDir.Count)
                value = FilesInDir.Count - 1;

            this.RaiseAndSetIfChanged(ref _selectedIndexInFileExplorer, value);
        }
    }

    public string CurrentPath
    {
        get => _currentPath;
        set => this.RaiseAndSetIfChanged(ref _currentPath, value);
    }

    public IStorageProvider? StorageProvider { get; init; }

    #endregion

    #region CTOR

    public FileExplorerViewModel()
    {
        PreviewImageViewModel = new PreviewImageViewModel();
        this.WhenAnyValue(x => x.CurrentPath)
            .Subscribe(GetFilesFromPath);
        CheckDirAndCreate(SettingsPath);
        CheckDirAndCreate(ThumbnailsPath);

        if (OperatingSystem.IsWindows())
        {
            CurrentPath = @"C:\oxford-iiit-pet\images";
        }

        if (OperatingSystem.IsLinux())
        {
            CurrentPath = "/home/arno/Downloads";
        }

        FileExplorerBackground = new SolidColorBrush(Colors.Transparent);
    }

    #endregion

    #region Private Methods

    public void StartPreview(string[] imagesInPath)
    {
        PreviewImageViewModel.StartPreview(imagesInPath);
        ScrollBarVisibility = ScrollBarVisibility.Disabled;
    }

    public void ClosePreview()
    {
        PreviewImageViewModel.ClosePreview();
        ScrollBarVisibility = ScrollBarVisibility.Auto;
    }

    private void GetFilesFromPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;
        if (path.Last() is '\\')
        {
            path = path[..^1];
        }

        var currentPathDirectoryInfo = new DirectoryInfo(path);
        if (!currentPathDirectoryInfo.Exists) return;
        if (_pathHistory.Count > 0 && currentPathDirectoryInfo.FullName == _pathHistory.Last()) return;
        FilesInDir.Clear();
        _cachedImagesPath = null;
        var dirs = currentPathDirectoryInfo.GetDirectories();
        var imagesFileInfo = currentPathDirectoryInfo.GetFiles()
            .Where(file => ImagesHelper.ImageFileExtensions.Contains(file.Extension.ToLower()))
            .OrderBy(f => f.Name, new NaturalSortComparer())
            .ToArray();
        SetDirsAndFiles(dirs, imagesFileInfo);
        _pathHistory.Add(currentPathDirectoryInfo.FullName);
    }

    private void SetDirsAndFiles(DirectoryInfo[] dirs, FileInfo[] imagesFileInfo)
    {
        // If dir not empty call SetDirs
        if (dirs is not {Length: 0})
            SetDirs(dirs);
        // If images not empty call SetFiles and set _imagesInPathCount
        if (imagesFileInfo is not {Length: 0})
        {
            SetFiles(imagesFileInfo);
            _imagesInPathCount = imagesFileInfo.Length;
        }
    }

    private void SetDirs(DirectoryInfo[] dirs)
    {
        foreach (var dir in dirs)
        {
            SetDir(dir);
        }
    }

    private async void SetFiles(FileInfo[] files)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            foreach (var file in files)
            {
                SetFile(file);
            }
        });
    }

    private async void SetDir(DirectoryInfo dir)
    {
        // if (dir.Parent?.FullName != CurrentPath)
        // {
        //     FilesInDir.Clear();
        //     return;
        // }
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var fileViewModel = new FileViewModel(dir.Parent?.FullName, dir.Name);
            fileViewModel.OnSendCommandToFileExplorer += HandleFileCommand;
            FilesInDir.Add(fileViewModel);
        });
    }


    private async void SetFile(FileInfo fileInfo)
    {
        // If we are not in CurrentPath, clear FilesInDir and return
        if (fileInfo.DirectoryName != CurrentPath)
        {
            FilesInDir.Clear();
            return;
        }

        // The path to the directory in the cache that contains thumbnails for this file
        var dirInCacheForCurrentFile = ThumbnailsPath + "\\" + fileInfo.Directory?.Name;
        // Check if dir exists, if not create it
        CheckDirAndCreate(dirInCacheForCurrentFile);
        // Get all files in the directory
        _cachedImagesPath ??= ImagesHelper.GetAllImagesInPathFromString(dirInCacheForCurrentFile);

        // Create an instance of the Bitmap object
        Bitmap? image;
        var thumbnailPath = _cachedImagesPath.FirstOrDefault(x => Path.GetFileName(x) == fileInfo.Name);
        // If cache is empty or the thumbnail can't be found in cache create a new thumbnail
        if (_cachedImagesPath is {Length: 0} || !File.Exists(thumbnailPath))
        {
            var outputFileInfo = new FileInfo(dirInCacheForCurrentFile + "\\" + fileInfo.Name);
            image = await GenerateBitmapThumb(fileInfo, outputFileInfo);
        }
        // Else (the thumbnail can be found in cache) use the thumbnail from cache
        else
        {
            image = await LoadImageAsync(thumbnailPath);
        }

        var fileViewModel = new FileViewModel(fileInfo.Directory?.FullName, fileInfo.Name, image);
        fileViewModel.OnSendCommandToFileExplorer += HandleFileCommand;
        FilesInDir.Add(fileViewModel);
    }

    public event EventHandler<Command> OnSendCommandToMainView;

    private void SendCommandToMainView(Command command)
    {
        OnSendCommandToMainView.Invoke(this, command);
    }

    private void HandleFileCommand(object? sender, Command command)
    {
        switch (command.Type)
        {
            case CommandType.ChangePath:
                ChangePath(command.Path);
                break;
            case CommandType.ClosePreview:
                ClosePreview();
                break;
            case CommandType.OpenFolderInNewTab:
            case CommandType.OpenImageInNewTab:
                SendCommandToMainView(command);
                break;
            case CommandType.StartPreview:
                if (command.ImagesInPath != null)
                    StartPreview(command.ImagesInPath);
                break;
        }
    }

    private static async Task<Bitmap> LoadImageAsync(string imagePath) => await Task.Run(() => new Bitmap(imagePath));

    private static async Task<Bitmap> GenerateBitmapThumb(FileInfo sourceFileInfo, FileInfo outputFileInfo)
    {
        using var image = new MagickImage(sourceFileInfo);
        image.Resize(new MagickGeometry(100, 100)
        {
            IgnoreAspectRatio = false
        });
        await image.WriteAsync(outputFileInfo);
        return await LoadImageAsync(outputFileInfo.FullName);
    }

    private async void OpenPathInFileExplorer()
    {
        if (StorageProvider is null) return;
        var result = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "Open Folder",
            AllowMultiple = false,
        });

        if (result.Count == 0) return;
        var path = result[0].Path;
        ChangePath(path.LocalPath);
    }


    private static void CheckDirAndCreate(string path)
    {
        var di = new DirectoryInfo(path);
        if (!di.Exists)
        {
            di.Create();
        }
    }

    public void GoToParentFolder()
    {
        var parent = Directory.GetParent(CurrentPath)?.FullName;
        ChangePath(parent);
    }

    #endregion

    #region Public Methods

    private void SendMessageToStatusBar(string message)
    {
        var command = new Command(CommandType.SendMessageToStatusBar, message: message);
        SendCommandToMainView(command);
    }

    public void ChangePath(string? folderPath)
    {
        if (folderPath is not null)
            CurrentPath = folderPath;
    }

    public void ToggleFileExplorer()
    {
        IsFileExplorerExpanded = !IsFileExplorerExpanded;
    }

    public void ToggleFileExplorerVisibility()
    {
        IsFileExplorerVisible = !IsFileExplorerVisible;
    }

    public void GoDown()
    {
        var isLastRow = SelectedIndex >= FilesInDir.Count - ColumnsCount;
        if (isLastRow)
        {
            SelectedIndex = FilesInDir.Count - 1;
        }
        else
        {
            SelectedIndex += ColumnsCount;
        }
    }

    public void GoUp()
    {
        var isFirstRow = SelectedIndex < ColumnsCount;
        if (isFirstRow) return;
        SelectedIndex -= ColumnsCount;
    }

    #endregion

    #region Public Commands

    public ICommand GotoParentFolderCommand => ReactiveCommand.Create(GoToParentFolder);
    public ICommand OpenFileCommand => ReactiveCommand.Create(OpenPathInFileExplorer);

    #endregion

    public void OpenTab()
    {
        var imagesInPath = ImagesHelper.GetAllImagesInPathFromFileViewModel(SelectedFile);
        var command = SelectedFile.IsImage
            ? new Command(CommandType.OpenImageInNewTab, imagesInPath, SelectedFile.FullPath, SelectedIndex)
            : new Command(CommandType.OpenFolderInNewTab, imagesInPath, SelectedFile.FullPath);

        SendCommandToMainView(command);
    }
}