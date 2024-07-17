using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Media;

namespace WaifuGallery.Models;

public class Settings
{
    #region Private Fields

    private static Settings? _instance;

    private static JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true, IgnoreReadOnlyFields = true, IgnoreReadOnlyProperties = true
    };

    private static string JsonPath => Path.Combine(SettingsPath, "Settings.json");

    #endregion

    #region Instance

    [JsonIgnore]
    public static Settings Instance
    {
        get
        {
            if (_instance is not null)
                return _instance;
            if (!File.Exists(JsonPath))
            {
                _instance = new Settings();
            }
            else
            {
                try
                {
                    _instance = JsonSerializer.Deserialize<Settings>(File.ReadAllText(JsonPath)) ??
                                new Settings();
                }
                catch
                {
                    _instance = new Settings();
                }
            }


            _instance.DefaultFont ??= FontManager.Current.DefaultFontFamily;
            if (_instance.ImagePreviewPreference.DefaultZoom is 0)
            {
                _instance.ImagePreviewPreference.DefaultZoom = 300;
            }

            if (_instance.AutoHideStatusBarDelay is 0)
            {
                _instance.AutoHideStatusBarDelay = 5000;
            }

            return _instance;
        }
    }

    #endregion

    #region Public Properties

    [JsonIgnore] public HotKeyManager HotKeyManager { get; init; } = new();
    public FileManagerPreference FileManagerPreference { get; init; } = new();
    public ImagePreviewPreference ImagePreviewPreference { get; init; } = new();
    public TabsPreference TabsPreference { get; init; } = new();
    public bool AutoHideStatusBar { get; set; }
    public bool ShouldHideMenuBar { get; set; }
    public bool ShouldHideStatusBar { get; set; }
    public bool ShouldHideTabsHeader { get; set; }
    public int AutoHideStatusBarDelay { get; set; }
    public string Theme { get; set; } = "System";

    public static string SettingsPath
    {
        get
        {
            if (OperatingSystem.IsWindows())
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "WaifuGallery");
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".WaifuGallery");
        }
    }

    public static string ThumbnailsPath => Path.Combine(SettingsPath, "Thumbnails");

    [JsonIgnore] public FontFamily? DefaultFont { get; set; }

    #endregion

    #region Public Methods

    public void Save()
    {
        HotKeyManager.SaveUserKeymap();
        var jsonPreference = JsonSerializer.Serialize(this, _jsonSerializerOptions);
        File.WriteAllText(JsonPath, jsonPreference);
    }

    #endregion
}

public class TabsPreference
{
    public bool IsDuplicateTabsAllowed { get; set; }
    public bool IsSettingsTabCycled { get; set; }
    public bool IsTabSettingsClosable { get; set; }
    public bool Loop { get; set; }
}

public class FileManagerPreference
{
    public bool ShouldCalculateFolderSize { get; set; }
    public bool ShouldSaveLastPathOnExit { get; set; }
    public bool ShouldHideFileManager { get; set; }
    public bool ShouldAskExtractionFolderName { get; set; }
    public string? FileManagerLastPath { get; set; }
    public int FileWidth { get; set; } = 170;
    public int FileHeight { get; set; } = 170;
}

public class ImagePreviewPreference
{
    public bool FollowMouse { get; set; }
    public bool Loop { get; set; }
    public int DefaultZoom { get; set; }
    public int Depth { get; set; }
}