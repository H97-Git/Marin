using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Styling;
using FluentAvalonia.Styling;
using ReactiveUI;
using WaifuGallery.Models;

namespace WaifuGallery.ViewModels.Tabs;

public class PreferencesTabViewModel : TabViewModelBase
{
    #region Private Fields

    private bool _autoHideStatusBar;
    private bool _isDuplicateTabsAllowed;
    private bool _isSettingsTabCycled;
    private bool _isTabSettingsClosable;
    private bool _previewFollowMouse;
    private bool _shouldAskExtractionFolderName;
    private bool _shouldCalculateFolderSize;
    private bool _shouldHideFileManager;
    private bool _shouldHideMenuBar;
    private bool _shouldHideStatusBar;
    private bool _shouldHideTabsHeader;
    private bool _shouldImageLoop;
    private bool _shouldImagePreviewLoop;
    private bool _shouldSaveLastPathOnExit;
    private int _autoHideStatusBarDelay;
    private int _previewDefaultZoom;
    private int _previewDepth;

    private string _currentThemeVariant = string.Empty;

    #endregion

    #region Private Methods

    private void InitializePreferences()
    {
        AutoHideStatusBar = Settings.Instance.AutoHideStatusBar;
        AutoHideStatusBarDelay = Settings.Instance.AutoHideStatusBarDelay;
        CurrentThemeVariant = Settings.Instance.Theme;
        IsDuplicateTabsAllowed = Settings.Instance.TabsPreference.IsDuplicateTabsAllowed;
        IsSettingsTabCycled = Settings.Instance.TabsPreference.IsSettingsTabCycled;
        IsTabSettingsClosable = Settings.Instance.TabsPreference.IsTabSettingsClosable;
        PreviewDefaultZoom = Settings.Instance.ImagePreviewPreference.DefaultZoom;
        PreviewDepth = Settings.Instance.ImagePreviewPreference.Depth;
        PreviewFollowMouse = Settings.Instance.ImagePreviewPreference.FollowMouse;
        ShouldAskExtractionFolderName = Settings.Instance.FileManagerPreference.ShouldAskExtractionFolderName;
        ShouldCalculateFolderSize = Settings.Instance.FileManagerPreference.ShouldCalculateFolderSize;
        ShouldHideFileManager = Settings.Instance.FileManagerPreference.ShouldHideFileManager;
        ShouldHideMenuBar = Settings.Instance.ShouldHideMenuBar;
        ShouldHideStatusBar = Settings.Instance.ShouldHideStatusBar;
        ShouldHideTabsHeader = Settings.Instance.ShouldHideTabsHeader;
        ShouldImagePreviewLoop = Settings.Instance.ImagePreviewPreference.Loop;
        ShouldImageLoop = Settings.Instance.TabsPreference.Loop;
        ShouldSaveLastPathOnExit = Settings.Instance.FileManagerPreference.ShouldSaveLastPathOnExit;
    }

    #endregion

    #region CTOR

    public PreferencesTabViewModel()
    {
        Id = Guid.Empty.ToString();
        Header = "Preferences";
        var fluentAvaloniaTheme = Application.Current?.Styles[0] as FluentAvaloniaTheme;
        InitializePreferences();
        this.WhenAnyValue(x => x.CurrentThemeVariant)
            .Subscribe(value =>
            {
                Settings.Instance.Theme = value;
                var themeVariant = value switch
                {
                    "Light" => ThemeVariant.Light,
                    "Dark" => ThemeVariant.Dark,
                    _ => ThemeVariant.Default
                };
                if (Application.Current != null)
                    Application.Current.RequestedThemeVariant = themeVariant;
                if (fluentAvaloniaTheme != null)
                    fluentAvaloniaTheme.PreferSystemTheme = value == "System";
            });
        this.WhenAnyValue(x => x.AutoHideStatusBar).Subscribe(value => Settings.Instance.AutoHideStatusBar = value);
        this.WhenAnyValue(x => x.AutoHideStatusBarDelay)
            .Subscribe(value => Settings.Instance.AutoHideStatusBarDelay = value);
        this.WhenAnyValue(x => x.IsDuplicateTabsAllowed)
            .Subscribe(value => Settings.Instance.TabsPreference.IsDuplicateTabsAllowed = value);
        this.WhenAnyValue(x => x.IsSettingsTabCycled)
            .Subscribe(value => Settings.Instance.TabsPreference.IsSettingsTabCycled = value);
        this.WhenAnyValue(x => x.IsTabSettingsClosable)
            .Subscribe(value => Settings.Instance.TabsPreference.IsTabSettingsClosable = value);
        this.WhenAnyValue(x => x.PreviewDefaultZoom)
            .Subscribe(value => Settings.Instance.ImagePreviewPreference.DefaultZoom = value);
        this.WhenAnyValue(x => x.PreviewDepth)
            .Subscribe(value => Settings.Instance.ImagePreviewPreference.Depth = value);
        this.WhenAnyValue(x => x.PreviewFollowMouse)
            .Subscribe(value => Settings.Instance.ImagePreviewPreference.FollowMouse = value);
        this.WhenAnyValue(x => x.ShouldAskExtractionFolderName).Subscribe(value =>
            Settings.Instance.FileManagerPreference.ShouldAskExtractionFolderName = value);
        this.WhenAnyValue(x => x.ShouldCalculateFolderSize).Subscribe(value =>
            Settings.Instance.FileManagerPreference.ShouldCalculateFolderSize = value);
        this.WhenAnyValue(x => x.ShouldHideFileManager).Subscribe(value =>
            Settings.Instance.FileManagerPreference.ShouldHideFileManager = value);
        this.WhenAnyValue(x => x.ShouldHideMenuBar).Subscribe(value => Settings.Instance.ShouldHideMenuBar = value);
        this.WhenAnyValue(x => x.ShouldHideStatusBar).Subscribe(value => Settings.Instance.ShouldHideStatusBar = value);
        this.WhenAnyValue(x => x.ShouldHideTabsHeader)
            .Subscribe(value => Settings.Instance.ShouldHideTabsHeader = value);
        this.WhenAnyValue(x => x.ShouldImagePreviewLoop)
            .Subscribe(value => Settings.Instance.ImagePreviewPreference.Loop = value);
        this.WhenAnyValue(x => x.ShouldSaveLastPathOnExit).Subscribe(value =>
            Settings.Instance.FileManagerPreference.ShouldSaveLastPathOnExit = value);
        this.WhenAnyValue(x => x.ShouldImageLoop)
            .Subscribe(value => Settings.Instance.TabsPreference.Loop = value);

        var groups = Settings.Instance.HotKeyManager.UserKeymap.GroupBy(x => x.Value);
        foreach (var group in groups)
        {
            var shortcut = new ShortcutViewModel(group.Key);
            foreach (var keyGestureKeyCommand in group)
            {
                shortcut.Gestures.Add(keyGestureKeyCommand.Key);
            }

            Shortcuts.Add(shortcut);
        }
    }

    #endregion

    #region Public Properties

    public ObservableCollection<ShortcutViewModel> Shortcuts { get; init; } = [];
    public string[] ThemesVariants { get; } = ["System", "Light", "Dark"];

    public string CurrentThemeVariant
    {
        get => _currentThemeVariant;
        set => this.RaiseAndSetIfChanged(ref _currentThemeVariant, value);
    }

    public bool IsDuplicateTabsAllowed
    {
        get => _isDuplicateTabsAllowed;
        set => this.RaiseAndSetIfChanged(ref _isDuplicateTabsAllowed, value);
    }

    public bool IsSettingsTabCycled
    {
        get => _isSettingsTabCycled;
        set => this.RaiseAndSetIfChanged(ref _isSettingsTabCycled, value);
    }

    public bool AutoHideStatusBar
    {
        get => _autoHideStatusBar;
        set => this.RaiseAndSetIfChanged(ref _autoHideStatusBar, value);
    }

    public int AutoHideStatusBarDelay
    {
        get => _autoHideStatusBarDelay;
        set => this.RaiseAndSetIfChanged(ref _autoHideStatusBarDelay, value);
    }

    public bool ShouldHideStatusBar
    {
        get => _shouldHideStatusBar;
        set => this.RaiseAndSetIfChanged(ref _shouldHideStatusBar, value);
    }

    public bool ShouldAskExtractionFolderName
    {
        get => _shouldAskExtractionFolderName;
        set => this.RaiseAndSetIfChanged(ref _shouldAskExtractionFolderName, value);
    }

    public bool ShouldImagePreviewLoop
    {
        get => _shouldImagePreviewLoop;
        set => this.RaiseAndSetIfChanged(ref _shouldImagePreviewLoop, value);
    }

    public bool ShouldImageLoop
    {
        get => _shouldImageLoop;
        set => this.RaiseAndSetIfChanged(ref _shouldImageLoop, value);
    }

    public bool ShouldHideFileManager
    {
        get => _shouldHideFileManager;
        set => this.RaiseAndSetIfChanged(ref _shouldHideFileManager, value);
    }

    public bool PreviewFollowMouse
    {
        get => _previewFollowMouse;
        set => this.RaiseAndSetIfChanged(ref _previewFollowMouse, value);
    }

    public bool ShouldHideMenuBar
    {
        get => _shouldHideMenuBar;
        set => this.RaiseAndSetIfChanged(ref _shouldHideMenuBar, value);
    }

    public bool ShouldHideTabsHeader
    {
        get => _shouldHideTabsHeader;
        set => this.RaiseAndSetIfChanged(ref _shouldHideTabsHeader, value);
    }

    public bool ShouldSaveLastPathOnExit
    {
        get => _shouldSaveLastPathOnExit;
        set => this.RaiseAndSetIfChanged(ref _shouldSaveLastPathOnExit, value);
    }

    public bool ShouldCalculateFolderSize
    {
        get => _shouldCalculateFolderSize;
        set => this.RaiseAndSetIfChanged(ref _shouldCalculateFolderSize, value);
    }

    public bool IsTabSettingsClosable
    {
        get => _isTabSettingsClosable;
        set => this.RaiseAndSetIfChanged(ref _isTabSettingsClosable, value);
    }

    public int PreviewDepth
    {
        get => _previewDepth;
        set => this.RaiseAndSetIfChanged(ref _previewDepth, value);
    }

    public int PreviewDefaultZoom
    {
        get => _previewDefaultZoom;
        set => this.RaiseAndSetIfChanged(ref _previewDefaultZoom, value);
    }

    public string CurrentVersion => "0.0.1";

    public string? CurrentAvaloniaVersion =>
        typeof(Application).Assembly.GetName().Version?.ToString();

    #endregion
}