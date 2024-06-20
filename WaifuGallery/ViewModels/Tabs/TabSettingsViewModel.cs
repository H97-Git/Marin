using System;
using ActiproSoftware.UI.Avalonia.Themes;
using Avalonia;
using Avalonia.Styling;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using WaifuGallery.Controls;

namespace WaifuGallery.ViewModels.Tabs;

public class TabSettingsViewModel : TabViewModelBase
{
    private readonly FluentAvaloniaTheme? _fluentAvaloniaTheme;

    // private readonly ModernTheme? _modernTheme;
    private string _currentThemeVariant = string.Empty;
    private bool _isDuplicateTabsAllowed = true;
    private bool _isSettingsTabCycled = true;

    public TabSettingsViewModel()
    {
        Id = Guid.Empty.ToString();
        Header = "Settings";
        _fluentAvaloniaTheme = Application.Current?.Styles[0] as FluentAvaloniaTheme;
        // _modernTheme = Application.Current?.Styles[1] as ModernTheme;
        IsDuplicateTabsAllowed = Preferences.Instance.IsDuplicateTabsAllowed;
        IsSettingsTabCycled = Preferences.Instance.IsSettingsTabCycled;
        CurrentThemeVariant = Preferences.Instance.Theme;
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

    public string CurrentVersion => "1.0.0";
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