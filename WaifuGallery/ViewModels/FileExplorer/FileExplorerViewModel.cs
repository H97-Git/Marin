﻿using System;
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

    private Brush? _fileExplorerBackground;
    private ObservableCollection<FileViewModel> _filesInDir = [];
    private ScrollBarVisibility _scrollBarVisibility = ScrollBarVisibility.Auto;
    private IStorageProvider _storageProvider;
    private bool _isFileExplorerExpanded = true;
    private bool _isFileExplorerVisible = true;
    private bool _isPointerOver;
    private bool _isSearchFocused;
    private int _columnsCount;
    private int _selectedIndexInFileExplorer;
    private string _currentPath = "";
    private string[]? _cachedImagesPath;
    private readonly FileExplorerHistory _pathHistory;

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

    #endregion

    #region Public Members

    public event EventHandler<Command>? OnSendCommandToMainView;
    public PreviewImageViewModel PreviewImageViewModel { get; init; }

    public FileViewModel SelectedFile => FilesInDir[SelectedIndex];

    public Brush? FileExplorerBackground
    {
        get => _fileExplorerBackground;
        set => this.RaiseAndSetIfChanged(ref _fileExplorerBackground, value);
    }

    public ObservableCollection<FileViewModel> FilesInDir
    {
        get => _filesInDir;
        set => this.RaiseAndSetIfChanged(ref _filesInDir, value);
    }

    public ScrollBarVisibility ScrollBarVisibility
    {
        get => _scrollBarVisibility;
        set => this.RaiseAndSetIfChanged(ref _scrollBarVisibility, value);
    }

    public bool IsPointerOver
    {
        get => _isPointerOver;
        set => this.RaiseAndSetIfChanged(ref _isPointerOver, value);
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

    public bool IsFileExplorerExpandedAndVisible => IsFileExplorerExpanded && IsFileExplorerVisible;

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

    #endregion

    #region CTOR

    public FileExplorerViewModel(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
        _pathHistory = new FileExplorerHistory();
        PreviewImageViewModel = new PreviewImageViewModel();
        PreviewImageViewModel.OnSendCommandToFileExplorer += HandlePreviewImageCommand;
        FileExplorerBackground = new SolidColorBrush(Colors.Transparent);

        this.WhenAnyValue(x => x.CurrentPath)
            .Subscribe(GetFilesFromPath);
        CheckDirAndCreate(SettingsPath);
        CheckDirAndCreate(ThumbnailsPath);

        if (OperatingSystem.IsWindows())
        {
            ChangePath(@"C:\oxford-iiit-pet\images");
        }

        if (OperatingSystem.IsLinux())
        {
            ChangePath(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
        }
    }

    #endregion

    #region Private Methods

    private async void GetFilesFromPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;
        //Remove last backslash if it exists
        if (path.Last() is '\\')
        {
            path = path[..^1];
        }

        var currentPathDirectoryInfo = new DirectoryInfo(path);
        if (!currentPathDirectoryInfo.Exists)
        {
            SendMessageToStatusBar("The path does not exist");
            return;
        }

        FilesInDir.Clear();
        _cachedImagesPath = null;
        await SetDirsAndFiles(currentPathDirectoryInfo);
    }

    private async Task SetDirsAndFiles(DirectoryInfo currentDir)
    {
        var dirs = currentDir.GetDirectories();
        var imagesFileInfo = currentDir.GetFiles()
            .Where(file => Helper.ImageFileExtensions.Contains(file.Extension.ToLower()))
            .OrderBy(f => f.Name, new NaturalSortComparer())
            .ToArray();
        // If dir not empty call SetDirs
        if (dirs is not {Length: 0})
            await SetDirs(dirs);
        // If images not empty call SetFiles and set _imagesInPathCount
        if (imagesFileInfo is not {Length: 0})
            await SetFiles(imagesFileInfo);
    }

    private async Task SetDirs(IEnumerable<DirectoryInfo> dirs)
    {
        foreach (var dir in dirs)
        {
            await SetDir(dir);
        }
    }

    private async Task SetFiles(IEnumerable<FileInfo> files)
    {
        foreach (var file in files)
        {
            await SetFile(file);
        }
    }

    private async Task SetDir(FileSystemInfo directoryInfo)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var fileViewModel = new FileViewModel(directoryInfo);
            fileViewModel.OnSendCommandToFileExplorer += HandleFileCommand;
            FilesInDir.Add(fileViewModel);
        });
    }

    private async Task SetFile(FileInfo fileInfo)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            // The path to the directory in the cache that contains thumbnails for this file
            var dirInCacheForCurrentFile = ThumbnailsPath + "\\" + fileInfo.Directory?.Name;
            // Check if dir exists, if not create it
            CheckDirAndCreate(dirInCacheForCurrentFile);
            // Get all files in the directory
            _cachedImagesPath ??= Helper.GetAllImagesInPathFromString(dirInCacheForCurrentFile);

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

            var fileViewModel = new FileViewModel(fileInfo, image);
            fileViewModel.OnSendCommandToFileExplorer += HandleFileCommand;
            FilesInDir.Add(fileViewModel);
        });
    }

    private void HandleFileCommand(object? sender, Command command)
    {
        switch (command.Type)
        {
            case CommandType.ChangePath:
                if (command.Path is null) return;
                ChangePath(command.Path);
                break;
            case CommandType.OpenFolderInNewTab:
            case CommandType.OpenImageInNewTab:
                SendCommandToMainView(command);
                break;
            case CommandType.StartPreview:
                if (command.ImagesInPath != null)
                    StartPreview(command.ImagesInPath);
                break;
            case CommandType.ClosePreview:
                ClosePreview();
                break;
        }
    }

    public void StartPreview(string[] imagesInPath)
    {
        PreviewImageViewModel.StartPreview(imagesInPath);
        ScrollBarVisibility = ScrollBarVisibility.Disabled;
    }

    public void ClosePreview()
    {
        PreviewImageViewModel.ClosePreview();
        ScrollBarVisibility = ScrollBarVisibility.Visible;
    }

    private void HandlePreviewImageCommand(object? sender, Command command)
    {
        switch (command.Type)
        {
            case CommandType.ToggleFileExplorerScrollBar:
                ScrollBarVisibility = ScrollBarVisibility.Auto;
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
        var result = await _storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "Open Folder",
            AllowMultiple = false,
        });

        if (result.Count is 0) return;
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

    private void SendMessageToStatusBar(string message)
    {
        var command = new Command(CommandType.SendMessageToStatusBar, message: message);
        SendCommandToMainView(command);
    }

    private void SendCommandToMainView(Command command)
    {
        OnSendCommandToMainView?.Invoke(this, command);
    }

    private void GoForwardHistory()
    {
        var path = _pathHistory.GoForward();
        UpdatePath(path);
    }

    private void GoBackwardHistory()
    {
        var path = _pathHistory.GoBack();
        UpdatePath(path);
    }

    private void UpdatePath(string path) => CurrentPath = path; //Dispatcher.UIThread.Post(() => );

    #endregion

    #region Public Methods

    public void GoToParentFolder()
    {
        var parent = Directory.GetParent(CurrentPath)?.FullName;
        if (parent is null) return;
        ChangePath(parent);
    }

    public void ChangePath(string path)
    {
        _pathHistory.AddPath(path);
        UpdatePath(path);
    }

    public void ToggleFileExplorer() => IsFileExplorerExpanded = !IsFileExplorerExpanded;

    public void ToggleFileExplorerVisibility() => IsFileExplorerVisible = !IsFileExplorerVisible;

    public void GoUp()
    {
        var isFirstRow = SelectedIndex < ColumnsCount;
        if (isFirstRow) return;
        SelectedIndex -= ColumnsCount;
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

    public void OpenImageTab()
    {
        var imagesInPath = Helper.GetAllImagesInPathFromFileViewModel(SelectedFile);
        var command = SelectedFile.IsImage
            ? new Command(CommandType.OpenImageInNewTab, imagesInPath, SelectedFile.FullPath, SelectedIndex)
            : new Command(CommandType.OpenFolderInNewTab, imagesInPath, SelectedFile.FullPath);

        SendCommandToMainView(command);
    }

    #endregion

    #region Public Commands

    public ICommand GotoParentFolderCommand => ReactiveCommand.Create(GoToParentFolder);
    public ICommand GotoNextDirCommand => ReactiveCommand.Create(GoForwardHistory);
    public ICommand GotoPreviousDirCommand => ReactiveCommand.Create(GoBackwardHistory);
    public ICommand OpenFileCommand => ReactiveCommand.Create(OpenPathInFileExplorer);

    #endregion
}