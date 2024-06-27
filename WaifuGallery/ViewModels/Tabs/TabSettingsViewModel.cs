using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Styling;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using WaifuGallery.Controls;

namespace WaifuGallery.ViewModels.Tabs;

public class TabSettingsViewModel : TabViewModelBase
{
    private Key _openSettingsKey = Key.None;
    private bool _isDuplicateTabsAllowed = true;
    private bool _isTabSettingsClosable;
    private bool _isSettingsTabCycled = true;
    private bool _shouldHideStatusBar = false;
    private bool _shouldHideFileExplorer = false;
    private bool _shouldHideMenuBar = false;
    private bool _shouldHideTabsHeader = false;
    private readonly FluentAvaloniaTheme? _fluentAvaloniaTheme;
    private string _currentThemeVariant = string.Empty;

    public TabSettingsViewModel()
    {
        Id = Guid.Empty.ToString();
        Header = "Settings";
        _fluentAvaloniaTheme = Application.Current?.Styles[0] as FluentAvaloniaTheme;
        CurrentThemeVariant = Preferences.Instance.Theme;
        IsDuplicateTabsAllowed = Preferences.Instance.IsDuplicateTabsAllowed;
        IsSettingsTabCycled = Preferences.Instance.IsSettingsTabCycled;
        IsTabSettingsClosable = Preferences.Instance.IsTabSettingsClosable;
        OpenSettingsKey = Preferences.Instance.OpenSettingsKey;
        ShouldHideMenuBar = Preferences.Instance.ShouldHideMenuBar;
        ShouldHideTabsHeader = Preferences.Instance.ShouldHideTabsHeader;
        ShouldHideFileExplorer = Preferences.Instance.ShouldHideFileExplorer;
        ShouldHideStatusBar = Preferences.Instance.ShouldHideStatusBar;
        this.WhenAnyValue(x => x.CurrentThemeVariant)
            .Subscribe(value =>
            {
                Preferences.Instance.Theme = value;
                var themeVariant = value switch
                {
                    "Light" => ThemeVariant.Light,
                    "Dark" => ThemeVariant.Dark,
                    _ => ThemeVariant.Default
                };
                if (Application.Current != null)
                    Application.Current.RequestedThemeVariant = themeVariant;
                if (_fluentAvaloniaTheme != null)
                    _fluentAvaloniaTheme.PreferSystemTheme = value == "System";
            });
        this.WhenAnyValue(x => x.IsDuplicateTabsAllowed)
            .Subscribe(value => Preferences.Instance.IsDuplicateTabsAllowed = value);
        this.WhenAnyValue(x => x.IsSettingsTabCycled)
            .Subscribe(value => Preferences.Instance.IsSettingsTabCycled = value);
        this.WhenAnyValue(x => x.IsTabSettingsClosable)
            .Subscribe(value => Preferences.Instance.IsTabSettingsClosable = value);
        this.WhenAnyValue(x => x.OpenSettingsKey)
            .Subscribe(value => Preferences.Instance.OpenSettingsKey = value);
        this.WhenAnyValue(x => x.ShouldHideMenuBar)
            .Subscribe(value => Preferences.Instance.ShouldHideMenuBar = value);
        this.WhenAnyValue(x => x.ShouldHideTabsHeader)
            .Subscribe(value => Preferences.Instance.ShouldHideTabsHeader = value);
        this.WhenAnyValue(x => x.ShouldHideFileExplorer)
            .Subscribe(value => Preferences.Instance.ShouldHideFileExplorer = value);
        this.WhenAnyValue(x => x.ShouldHideStatusBar)
            .Subscribe(value => Preferences.Instance.ShouldHideStatusBar = value);
    }

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

    public bool IsTabSettingsClosable
    {
        get => _isTabSettingsClosable;
        set => this.RaiseAndSetIfChanged(ref _isTabSettingsClosable, value);
    }

    public Key OpenSettingsKey
    {
        get => _openSettingsKey;
        set => this.RaiseAndSetIfChanged(ref _openSettingsKey, value);
    }

    public string CurrentVersion => "0.0.1";
    // typeof(Program).Assembly.GetName().Version?.ToString();

    public string CurrentAvaloniaVersion =>
        typeof(Application).Assembly.GetName().Version?.ToString();


    public async void ShowDialogAsync()
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
}