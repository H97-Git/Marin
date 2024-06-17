using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using ReactiveUI;

namespace WaifuGallery.ViewModels;

public class Preferences : ReactiveObject
{
    private static Preferences? _instances = null;

    private string _theme = "Light";
    private FontFamily _defaultFont;
    private double _defaultFontSize;

    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "WaifuGallery",
        "settings.json");

    public string Theme
    {
        get => _theme;
        set => this.RaiseAndSetIfChanged(ref _theme, value);
    }

    public double DefaultFontSize
    {
        get => _defaultFontSize;
        set => this.RaiseAndSetIfChanged(ref _defaultFontSize, value);
    }


    [JsonIgnore]
    public FontFamily DefaultFont
    {
        get => _defaultFont;
        set => this.RaiseAndSetIfChanged(ref _defaultFont, value);
    }

    [JsonIgnore]
    public static Preferences Instance
    {
        get
        {
            if (_instances == null)
            {
                if (!File.Exists(SettingsPath))
                {
                    _instances = new Preferences();
                }
                else
                {
                    try
                    {
                        _instances = JsonSerializer.Deserialize<Preferences>(File.ReadAllText(SettingsPath)) ??
                                     new Preferences();
                    }
                    catch
                    {
                        _instances = new Preferences();
                    }
                }
            }

            return _instances;
        }
    }


    public void Save()
    {
        var dir = Path.GetDirectoryName(SettingsPath);
        if (dir == null) return;
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllText(SettingsPath,
            JsonSerializer.Serialize(this, new JsonSerializerOptions {WriteIndented = true}));
    }
}