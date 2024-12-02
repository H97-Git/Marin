using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Media;
using Marin.UI.ViewModels.Tabs;
using Serilog;

namespace Marin.UI.Models;

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

            Log.Debug("Settings instance not found...");
            var isSettingsLoadedFromJson = false;
            if (!File.Exists(JsonPath))
            {
                Log.Debug("Settings file not found. Creating new settings...");
                _instance = new Settings();
            }
            else
            {
                Log.Debug("Settings file found. Loading settings...");
                _instance = JsonSerializer.Deserialize<Settings>(File.ReadAllText(JsonPath));
                if (_instance is not null)
                {
                    isSettingsLoadedFromJson = true;
                }
                else
                {
                    Log.Warning("Failed to deserialize settings. Creating new settings...");
                    _instance = new Settings();
                }
            }


            if (isSettingsLoadedFromJson) return _instance;
            _instance.DefaultFont ??= FontManager.Current.DefaultFontFamily;
            if (_instance.ImagePreviewPreference.DefaultZoom is 0)
            {
                _instance.ImagePreviewPreference.DefaultZoom = 300;
            }

            if (_instance.StatusBarPreference.AutoHideStatusBarDelay is 0)
            {
                _instance.StatusBarPreference.AutoHideStatusBarDelay = 5000;
            }

            return _instance;
        }
    }

    #endregion

    #region Public Properties

    [JsonIgnore] public HotKeyManager HotKeyManager { get; init; } = new();
    public FileManagerPreference FileManagerPreference { get; init; } = new();
    public ImagePreviewPreference ImagePreviewPreference { get; init; } = new();
    public StatusBarPreference StatusBarPreference { get; init; } = new();
    public TabsPreference TabsPreference { get; init; } = new();
    public bool ShouldHideMenuBar { get; set; }
    public string Theme { get; set; } = "System";
    public bool OpenPreferencesOnStartup { get; set; }
    public bool SaveLastSessionOnExit { get; set; }
    public bool LoadLastSessionOnStartUp { get; set; }

    public static string SettingsPath
    {
        get
        {
            if (OperatingSystem.IsWindows())
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Marin");
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".Marin");
        }
    }

    public static string SessionsPath => Path.Combine(SettingsPath, "Sessions");
    public static string LogsPath => Path.Combine(SettingsPath, "Logs");
    public static string LastSessionsPath => Path.Combine(SessionsPath, "Last.json");
    public static string ThumbnailsPath => Path.Combine(SettingsPath, "Thumbnails");
    [JsonIgnore] public FontFamily? DefaultFont { get; set; }

    #endregion

    #region Public Methods

    public void Save()
    {
        Log.Debug("Saving settings and hotkeys...");
        if (Instance.SaveLastSessionOnExit)
        {
            SaveSession();
        }

        HotKeyManager.SaveUserKeymap();
        var jsonPreference = JsonSerializer.Serialize(this, _jsonSerializerOptions);
        File.WriteAllText(JsonPath, jsonPreference);
    }

    public static void SaveSession(string sessionName = "Last")
    {
        Log.Debug("Saving session... {SessionName}", sessionName);
        var openTabs = App.GetOpenTabs() ?? [];
        var list = new List<string>();
        foreach (var tab in openTabs)
        {
            if (tab is ImageTabViewModel imageTabViewModel)
                list.Add(imageTabViewModel.CurrentImagePath);
        }

        var json = JsonSerializer.Serialize(list, _jsonSerializerOptions);
        Directory.CreateDirectory(SessionsPath);
        File.WriteAllText(Path.Combine(SessionsPath, $"{sessionName}.json"), json);
    }

    #endregion
}

public class TabsPreference
{
    public bool IsDuplicateTabsAllowed { get; set; }
    public bool IsSettingsTabCycled { get; set; }
    public bool IsTabSettingsClosable { get; set; }
    public bool Loop { get; set; }
    public bool ShouldHideTabsHeader { get; set; }

    [JsonIgnore] public List<string> LastSession { get; set; }
}

public class StatusBarPreference
{
    public bool AutoHideStatusBar { get; set; }
    public bool ShouldHideStatusBar { get; set; }
    public int AutoHideStatusBarDelay { get; set; }
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
    public string? Position { get; set; }
    public string? DefaultSortOrder { get; set; }
}

public class ImagePreviewPreference
{
    public bool FollowMouse { get; set; }
    public bool Loop { get; set; }
    public int DefaultZoom { get; set; }
    public int Depth { get; set; }
}