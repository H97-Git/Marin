using System;
using System.IO;
using System.Text;
using System.Text.Json;
using ActiproSoftware.UI.Avalonia.Themes;
using Avalonia;
using Avalonia.Styling;
using FluentAvalonia.Styling;
using ReactiveUI;

namespace WaifuGallery.ViewModels.Tabs;

public class TabSettingsViewModel : TabViewModelBase
{
    private readonly FluentAvaloniaTheme? _fluentAvaloniaTheme;
    private readonly ModernTheme? _modernTheme;
    private string _currentThemeVariant;
    private static Preferences Preferences => Preferences.Instance;

    public TabSettingsViewModel(Guid id)
    {
        Id = id;
        Header = "Settings";
        _fluentAvaloniaTheme = Application.Current?.Styles[0] as FluentAvaloniaTheme;
        _modernTheme = Application.Current?.Styles[1] as ModernTheme;
    }

    public string[] ThemesVariants { get; } = ["System", "Light", "Dark"];


    public string CurrentThemeVariant
    {
        get => _currentThemeVariant;
        set
        {
            var n = value switch
            {
                "Light" => ThemeVariant.Light,
                "Dark" => ThemeVariant.Dark,
                _ => ThemeVariant.Default
            };
            if (Application.Current != null)
                Application.Current.RequestedThemeVariant = n;
            if (_fluentAvaloniaTheme != null)
                _fluentAvaloniaTheme.PreferSystemTheme = value == "System";
            Preferences.Instance.Theme = value;

            this.RaiseAndSetIfChanged(ref _currentThemeVariant, value);
        }
    }

    public string CurrentVersion => "1.0.0";
    // typeof(Program).Assembly.GetName().Version?.ToString();

    public string CurrentAvaloniaVersion =>
        typeof(Application).Assembly.GetName().Version?.ToString();
}