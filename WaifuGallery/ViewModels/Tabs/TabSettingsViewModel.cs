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
    private const string FileName = "settings.json";
    private const string Dark = "Dark";
    private const string Light = "Light";
    private const string System = "System";
    private readonly FluentAvaloniaTheme _fluentAvaloniaTheme;
    private readonly ModernTheme _modernTheme;
    private string _currentAppTheme;

    public class ThemeSettings
    {
        public string TSAppTheme { get; set; }
    }

    public TabSettingsViewModel(Guid id)
    {
        Id = id;
        Header = "Settings";
        _fluentAvaloniaTheme = Application.Current.Styles[0] as FluentAvaloniaTheme;
        _modernTheme = Application.Current.Styles[1] as ModernTheme;
        LoadSettings();
    }

    public string[] AppThemes { get; } = [System, Light, Dark];

    private void SetDefaultSettings()
    {
        CurrentAppTheme = Dark;
    }

    private static string SettingsPath
    {
        get
        {
            if (OperatingSystem.IsWindows())
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\WaifuGallery";
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.WaifuGallery";
        }
    }

    public void LoadSettings()
    {
        try
        {
            var options = new JsonSerializerOptions();
            var path = Path.Combine(SettingsPath, FileName);

            if (!File.Exists(path))
            {
                SetDefaultSettings();
                return;
            }

            var jsonString = File.ReadAllText(path);
            var settings = JsonSerializer.Deserialize<ThemeSettings>(jsonString, options);

            if (settings is null) return;
            CurrentAppTheme = settings.TSAppTheme;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public void SaveSettings()
    {
        try
        {
            var settings = new ThemeSettings
            {
                TSAppTheme = CurrentAppTheme,
            };

            var options = new JsonSerializerOptions();
            var jsonString = JsonSerializer.Serialize(settings, options);
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(directory, FileName);
            File.WriteAllText(path, jsonString, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public string CurrentAppTheme
    {
        get => _currentAppTheme;
        set
        {
            var newTheme = GetThemeVariant(value);
            if (Application.Current != null) Application.Current.RequestedThemeVariant = newTheme;
            _fluentAvaloniaTheme.PreferSystemTheme = value == System;

            this.RaiseAndSetIfChanged(ref _currentAppTheme, value);
        }
    }

    private static ThemeVariant GetThemeVariant(string value)
    {
        return value switch
        {
            Light => ThemeVariant.Light,
            Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };
    }

    public string CurrentVersion => "1.0.0";
    // typeof(Program).Assembly.GetName().Version?.ToString();

    public string CurrentAvaloniaVersion =>
        typeof(Application).Assembly.GetName().Version?.ToString();
}