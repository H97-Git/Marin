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
    private bool _shouldCalculateFolderSize;
    private bool _shouldHideFileManager;
    private bool _shouldHideMenuBar;
    private bool _shouldHideStatusBar;
    private bool _shouldHideTabsHeader;
    private bool _shouldSaveLastPathOnExit;
    private int _previewDefaultZoom;
    private int _previewDepth;
   
    private string _currentThemeVariant = string.Empty;

    #endregion

    #region Private Methods

    private void InitializePreferences()
    {
        CurrentThemeVariant = Settings.Instance.Theme;
        IsDuplicateTabsAllowed = Settings.Instance.IsDuplicateTabsAllowed;
        IsSettingsTabCycled = Settings.Instance.IsSettingsTabCycled;
        IsTabSettingsClosable = Settings.Instance.IsTabSettingsClosable;
        ShouldHideMenuBar = Settings.Instance.ShouldHideMenuBar;
        ShouldHideTabsHeader = Settings.Instance.ShouldHideTabsHeader;
        ShouldHideFileManager = Settings.Instance.ShouldHideFileManager;
        ShouldHideStatusBar = Settings.Instance.ShouldHideStatusBar;
        ShouldSaveLastPathOnExit = Settings.Instance.ShouldSaveLastPathOnExit;
        ShouldCalculateFolderSize = Settings.Instance.ShouldCalculateFolderSize;
        PreviewDepth = Settings.Instance.PreviewDepth;
        AutoHideStatusBar = Settings.Instance.AutoHideStatusBar;
        PreviewDefaultZoom = Settings.Instance.PreviewDefaultZoom;
        PreviewFollowMouse = Settings.Instance.PreviewFollowMouse;
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
        this.WhenAnyValue(x => x.IsDuplicateTabsAllowed)
            .Subscribe(value => Settings.Instance.IsDuplicateTabsAllowed = value);
        this.WhenAnyValue(x => x.IsSettingsTabCycled)
            .Subscribe(value => Settings.Instance.IsSettingsTabCycled = value);
        this.WhenAnyValue(x => x.IsTabSettingsClosable)
            .Subscribe(value => Settings.Instance.IsTabSettingsClosable = value);
        this.WhenAnyValue(x => x.ShouldHideMenuBar)
            .Subscribe(value => Settings.Instance.ShouldHideMenuBar = value);
        this.WhenAnyValue(x => x.ShouldHideTabsHeader)
            .Subscribe(value => Settings.Instance.ShouldHideTabsHeader = value);
        this.WhenAnyValue(x => x.ShouldHideFileManager)
            .Subscribe(value => Settings.Instance.ShouldHideFileManager = value);
        this.WhenAnyValue(x => x.ShouldHideStatusBar)
            .Subscribe(value => Settings.Instance.ShouldHideStatusBar = value);
        this.WhenAnyValue(x => x.ShouldSaveLastPathOnExit)
            .Subscribe(value => Settings.Instance.ShouldSaveLastPathOnExit = value);
        this.WhenAnyValue(x => x.PreviewDepth)
            .Subscribe(value => Settings.Instance.PreviewDepth = value);
        this.WhenAnyValue(x => x.ShouldCalculateFolderSize)
            .Subscribe(value => Settings.Instance.ShouldCalculateFolderSize = value);
        this.WhenAnyValue(x => x.AutoHideStatusBar)
            .Subscribe(value => Settings.Instance.AutoHideStatusBar = value);
        this.WhenAnyValue(x => x.PreviewDefaultZoom)
            .Subscribe(value => Settings.Instance.PreviewDefaultZoom = value);
        this.WhenAnyValue(x => x.PreviewFollowMouse)
            .Subscribe(value => Settings.Instance.PreviewFollowMouse = value);
       
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

    public bool ShouldHideStatusBar
    {
        get => _shouldHideStatusBar;
        set => this.RaiseAndSetIfChanged(ref _shouldHideStatusBar, value);
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