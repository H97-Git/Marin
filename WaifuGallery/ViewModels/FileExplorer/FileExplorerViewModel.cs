using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using WaifuGallery.Commands;
using WaifuGallery.Helpers;
using WaifuGallery.Models;

namespace WaifuGallery.ViewModels.FileExplorer;

public class FileExplorerViewModel : ViewModelBase
{
    #region Private Fields

    private Brush? _fileExplorerBackground;
    private ObservableCollection<FileViewModel> _filesInDir = [];
    private ScrollBarVisibility _scrollBarVisibility = ScrollBarVisibility.Auto;
    private bool _isFileExplorerExpanded = true;
    private bool _isFileExplorerVisible = true;
    private bool _isPointerOver;
    private bool _isSearchFocused;
    private int _columnsCount;
    private int _selectedIndexInFileExplorer;
    private readonly FileExplorerHistory _pathHistory = new();
    private string _currentPath = string.Empty;

    #endregion

    #region Private Properties

    private static string SettingsPath
    {
        get
        {
            if (OperatingSystem.IsWindows())
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "WaifuGallery");
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".WaifuGallery");
        }
    }

    #endregion

    #region Private Methods

    private void GetFilesFromPath(string? path)
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
            SendMessageToStatusBar(InfoBarSeverity.Error, "The path does not exist");
            return;
        }

        Settings.Instance.FileExplorerLastPath = path;
        FilesInDir.Clear();
        SetDirsAndFiles(currentPathDirectoryInfo);
    }

    private void SetDirsAndFiles(DirectoryInfo currentDir)
    {
        var dirs = currentDir.GetDirectories().Where(f => f.Name.First() is not '.' && f.Name.First() is not '$')
            .ToArray();
        var imagesFileInfo = currentDir.GetFiles()
            .Where(file => Helper.AllFileExtensions.Contains(file.Extension.ToLower()))
            .OrderBy(f => f.Name, new NaturalSortComparer())
            .ToArray();
        // If dir not empty call SetDirs
        if (dirs is not {Length: 0})
            SetDirs(dirs);
        // If images not empty call SetFiles and set _imagesInPathCount
        if (imagesFileInfo is not {Length: 0})
            SetFiles(imagesFileInfo);
    }

    private void SetDirs(IEnumerable<DirectoryInfo> dirs)
    {
        foreach (var dir in dirs)
        {
            Dispatcher.UIThread.Post(() => FilesInDir.Add(new FileViewModel(dir)));
        }
    }

    private void SetFiles(IEnumerable<FileInfo> files)
    {
        foreach (var file in files)
        {
            Dispatcher.UIThread.Post(() => FilesInDir.Add(new FileViewModel(file)));
        }
    }

    private async void OpenPathInFileExplorer()
    {
        var storageProvider = App.GetTopLevel()?.StorageProvider;
        if (storageProvider is null) return;
        var result = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "Open Folder",
            AllowMultiple = false,
        });

        if (result.Count is 0) return;
        var path = result[0].Path;
        ChangePath(path.LocalPath);
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

    private void UpdatePath(string path) => CurrentPath = path;

    #endregion

    #region CTOR

    public FileExplorerViewModel()
    {
        FileExplorerBackground = new SolidColorBrush(Colors.Transparent);
        this.WhenAnyValue(x => x.CurrentPath)
            .Subscribe(GetFilesFromPath);
        this.WhenAnyValue(x => x.IsFileExplorerExpanded)
            .Subscribe(_ =>
                FileExplorerBackground = IsFileExplorerExpanded ? new SolidColorBrush(Colors.Transparent) : null);
        this.WhenAnyValue(x => x.PreviewImageViewModel.IsPreviewImageVisible)
            .Subscribe(b => ScrollBarVisibility = b ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto);

        if (Settings.Instance.ShouldSaveLastPathOnExit && Settings.Instance.FileExplorerLastPath is not null)
        {
            ChangePath(Settings.Instance.FileExplorerLastPath);
        }
        else
        {
            if (OperatingSystem.IsWindows())
            {
                ChangePath(@"C:\oxford-iiit-pet\images");
            }

            if (OperatingSystem.IsLinux())
            {
                ChangePath(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            }
        }

        MessageBus.Current.Listen<ChangePathCommand>().Subscribe(x => ChangePath(x.Path));
        MessageBus.Current.Listen<StartPreviewCommand>().Subscribe(x => StartPreview(x.Path));
        MessageBus.Current.Listen<ToggleFileExplorerCommand>().Subscribe(_ => ToggleFileExplorer());
        MessageBus.Current.Listen<ToggleFileExplorerVisibilityCommand>().Subscribe(_ => ToggleFileExplorerVisibility());
        MessageBus.Current.Listen<RefreshFileExplorerCommand>().Subscribe(_ => GetFilesFromPath(CurrentPath));
    }

    #endregion

    #region Public Properties

    public PreviewImageViewModel PreviewImageViewModel { get; init; } = new();

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
        set => this.RaiseAndSetIfChanged(ref _isFileExplorerExpanded, value);
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

    #region Public Methods

    public void StartPreview(string path)
    {
        PreviewImageViewModel.StartPreview(path);
        ScrollBarVisibility = ScrollBarVisibility.Disabled;
    }

    public void ClosePreview()
    {
        PreviewImageViewModel.ClosePreview();
        ScrollBarVisibility = ScrollBarVisibility.Visible;
    }

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

    public void OpenImageTabFromKeyboardEvent()
    {
        var imagesInPath = Helper.GetAllImagesInPath(SelectedFile);
        var command = SelectedFile.IsImage
            ? new OpenInNewTabCommand(SelectedIndex, imagesInPath)
            : new OpenInNewTabCommand(0, imagesInPath);

        MessageBus.Current.SendMessage(command);
    }

    #endregion

    #region Public Commands

    public ICommand GotoParentFolderCommand => ReactiveCommand.Create(GoToParentFolder);
    public ICommand GotoNextDirCommand => ReactiveCommand.Create(GoForwardHistory);
    public ICommand GotoPreviousDirCommand => ReactiveCommand.Create(GoBackwardHistory);
    public ICommand OpenFileCommand => ReactiveCommand.Create(OpenPathInFileExplorer);

    public ICommand Paste =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new PasteCommand(CurrentPath)); });

    public ICommand CreateNewFolder =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new NewFolderCommand(CurrentPath)); });

    public ICommand OpenInExplorer =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new OpenInFileExplorerCommand(CurrentPath)); });

    #endregion
}