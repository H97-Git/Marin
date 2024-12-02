using System;
using Avalonia;
using Avalonia.Styling;
using FluentAvalonia.Styling;
using Marin.UI.Models;
using ReactiveUI;

namespace Marin.UI.ViewModels.Tabs.Preferences;

public class GeneralPreferencesViewModel : ViewModelBase
{
    private bool _isVisible;
    public string[] ThemesVariants { get; } = ["System", "Light", "Dark"];
    private string _currentThemeVariant = string.Empty;
    private bool _shouldLoadLastSessionOnStartUp;
    private bool _shouldOpenPreferencesOnStartUp;
    private bool _shouldSaveLastSessionOnExit;

    public GeneralPreferencesViewModel()
    {
        CurrentThemeVariant = Settings.Instance.Theme;
        var fluentAvaloniaTheme = Application.Current?.Styles[0] as FluentAvaloniaTheme;
        ShouldOpenPreferencesOnStartUp = Settings.Instance.OpenPreferencesOnStartup;
        ShouldLoadLastSessionOnStartUp = Settings.Instance.LoadLastSessionOnStartUp;
        ShouldSaveLastSessionOnExit = Settings.Instance.SaveLastSessionOnExit;
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
        this.WhenAnyValue(x => x.ShouldOpenPreferencesOnStartUp).Subscribe(value =>
            Settings.Instance.OpenPreferencesOnStartup = value);
        this.WhenAnyValue(x => x.ShouldSaveLastSessionOnExit)
            .Subscribe(value => Settings.Instance.SaveLastSessionOnExit = value);
        this.WhenAnyValue(x => x.ShouldLoadLastSessionOnStartUp)
            .Subscribe(value => Settings.Instance.LoadLastSessionOnStartUp = value);
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }

    public string CurrentThemeVariant
    {
        get => _currentThemeVariant;
        set => this.RaiseAndSetIfChanged(ref _currentThemeVariant, value);
    }

    public bool ShouldOpenPreferencesOnStartUp
    {
        get => _shouldOpenPreferencesOnStartUp;
        set => this.RaiseAndSetIfChanged(ref _shouldOpenPreferencesOnStartUp, value);
    }

    public bool ShouldSaveLastSessionOnExit
    {
        get => _shouldSaveLastSessionOnExit;
        set => this.RaiseAndSetIfChanged(ref _shouldSaveLastSessionOnExit, value);
    }

    public bool ShouldLoadLastSessionOnStartUp
    {
        get => _shouldLoadLastSessionOnStartUp;
        set => this.RaiseAndSetIfChanged(ref _shouldLoadLastSessionOnStartUp, value);
    }
}