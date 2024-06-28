using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Input;
using Avalonia.Media;

namespace WaifuGallery.ViewModels;

public class Settings
{
    #region Private Fields

    private static Settings? _instance;

    private static readonly string SettingsPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "WaifuGallery",
            "settings.json");

    #endregion

    #region Instance

    [JsonIgnore]
    public static Settings Instance
    {
        get
        {
            if (_instance != null) return _instance;
            if (!File.Exists(SettingsPath))
            {
                _instance = new Settings();
            }
            else
            {
                try
                {
                    _instance = JsonSerializer.Deserialize<Settings>(File.ReadAllText(SettingsPath)) ??
                                new Settings();
                }
                catch
                {
                    _instance = new Settings();
                }
            }

            if (_instance.DefaultFont == null)
            {
                _instance.DefaultFont = FontManager.Current.DefaultFontFamily;
            }

            return _instance;
        }
    }

    #endregion

    #region Public Properties

    public Key OpenSettingsKey { get; set; } = Key.F1;
    public bool IsDuplicateTabsAllowed { get; set; }
    public bool IsSettingsTabCycled { get; set; }
    public bool IsTabSettingsClosable { get; set; }
    public bool ShouldHideFileExplorer { get; set; }
    public bool ShouldHideMenuBar { get; set; }
    public bool ShouldHideStatusBar { get; set; }
    public bool ShouldHideTabsHeader { get; set; }
    public bool ShouldSaveLastPathOnExit { get; set; }
    public int PreviewDepth { get; set; }
    public string Theme { get; set; } = "System";
    public string? FileExplorerLastPath { get; set; }

    [JsonIgnore] public FontFamily? DefaultFont { get; set; }

    #endregion

    #region Public Methods

    public void Save()
    {
        var json = JsonSerializer.Serialize(this,
            //This warning can be ignored since this method (as of now) is only called once when the app exits.
#pragma warning disable CA1869
            new JsonSerializerOptions
#pragma warning restore CA1869
                {WriteIndented = true, IgnoreReadOnlyFields = true, IgnoreReadOnlyProperties = true});
        File.WriteAllText(SettingsPath, json);
    }

    #endregion
}