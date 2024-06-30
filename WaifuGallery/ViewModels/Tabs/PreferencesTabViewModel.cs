using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Styling;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using WaifuGallery.Controls;
using WaifuGallery.ViewModels.Dialogs;

namespace WaifuGallery.ViewModels.Tabs;

public class PreferencesTabViewModel : TabViewModelBase
{
    #region Private Fields

    private Key _openSettingsKey = Key.None;
    private bool _isDuplicateTabsAllowed;
    private bool _isTabSettingsClosable;
    private bool _isSettingsTabCycled;
    private bool _shouldHideStatusBar;
    private bool _shouldHideFileExplorer;
    private bool _shouldHideMenuBar;
    private bool _shouldHideTabsHeader;
    private bool _shouldSaveLastPathOnExit;
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
        OpenSettingsKey = Settings.Instance.OpenSettingsKey;
        ShouldHideMenuBar = Settings.Instance.ShouldHideMenuBar;
        ShouldHideTabsHeader = Settings.Instance.ShouldHideTabsHeader;
        ShouldHideFileExplorer = Settings.Instance.ShouldHideFileExplorer;
        ShouldHideStatusBar = Settings.Instance.ShouldHideStatusBar;
        ShouldSaveLastPathOnExit = Settings.Instance.ShouldSaveLastPathOnExit;
        PreviewDepth = Settings.Instance.PreviewDepth;
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
        this.WhenAnyValue(x => x.OpenSettingsKey)
            .Subscribe(value => Settings.Instance.OpenSettingsKey = value);
        this.WhenAnyValue(x => x.ShouldHideMenuBar)
            .Subscribe(value => Settings.Instance.ShouldHideMenuBar = value);
        this.WhenAnyValue(x => x.ShouldHideTabsHeader)
            .Subscribe(value => Settings.Instance.ShouldHideTabsHeader = value);
        this.WhenAnyValue(x => x.ShouldHideFileExplorer)
            .Subscribe(value => Settings.Instance.ShouldHideFileExplorer = value);
        this.WhenAnyValue(x => x.ShouldHideStatusBar)
            .Subscribe(value => Settings.Instance.ShouldHideStatusBar = value);
        this.WhenAnyValue(x => x.ShouldSaveLastPathOnExit)
            .Subscribe(value => Settings.Instance.ShouldSaveLastPathOnExit = value);
        this.WhenAnyValue(x => x.PreviewDepth)
            .Subscribe(value => Settings.Instance.PreviewDepth = value);
    }

    #endregion

    #region Public Properties

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

    public bool ShouldHideStatusBar
    {
        get => _shouldHideStatusBar;
        set => this.RaiseAndSetIfChanged(ref _shouldHideStatusBar, value);
    }

    public bool ShouldHideFileExplorer
    {
        get => _shouldHideFileExplorer;
        set => this.RaiseAndSetIfChanged(ref _shouldHideFileExplorer, value);
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

    public Key OpenSettingsKey
    {
        get => _openSettingsKey;
        set => this.RaiseAndSetIfChanged(ref _openSettingsKey, value);
    }

    public string CurrentVersion => "0.0.1";

    public string? CurrentAvaloniaVersion =>
        typeof(Application).Assembly.GetName().Version?.ToString();

    #endregion

    #region Public Methods

    public async void ShowSetKeyDialogAsync()
    {
        var dialog = new ContentDialog()
        {
            Title = "Press any key",
            PrimaryButtonText = "Ok",
            SecondaryButtonText = "Cancel",
        };

        var viewModel = new KeyboardKeySetterViewModel();
        dialog.Content = new KeyboardKeySetter()
        {
            DataContext = viewModel
        };

        _ = await dialog.ShowAsync();
    }

    #endregion
}