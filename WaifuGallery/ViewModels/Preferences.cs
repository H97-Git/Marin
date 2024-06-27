using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Input;
using Avalonia.Media;

namespace WaifuGallery.ViewModels;

public class Preferences
{
    // private FontFamily _defaultFont;
    // private bool _isDuplicateTabsAllowed;
    // private bool _isSettingsTabCycled;
    // private double _defaultFontSize;
    private static Preferences? _instances;
    // private string _theme = "Light";

    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "WaifuGallery",
        "settings.json");

    public Key OpenSettingsKey { get; set; } = Key.F1;
    public string Theme { get; set; }
    public bool IsSettingsTabCycled { get; set; }
    public bool IsTabSettingsClosable { get; set; }
    public bool IsDuplicateTabsAllowed { get; set; }
    public bool ShouldHideStatusBar { get; set; }
    public bool ShouldHideFileExplorer { get; set; }
    public bool ShouldHideMenuBar { get; set; }
    public bool ShouldHideTabsHeader { get; set; }
    public double DefaultFontSize { get; set; }

    [JsonIgnore] public FontFamily DefaultFont { get; set; }

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
        var json = JsonSerializer.Serialize(this,
            new JsonSerializerOptions
                {WriteIndented = true, IgnoreReadOnlyFields = true, IgnoreReadOnlyProperties = true});
        File.WriteAllText(SettingsPath, json);
    }
}