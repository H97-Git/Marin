using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Avalonia.Input;
using Serilog;
using WaifuGallery.Converters;
using WaifuGallery.Helpers;

namespace WaifuGallery.Models;

public enum KeyCommand
{
    None,
    FirstImage,
    LastImage,
    FitToHeightAndResetZoom,
    FitToWidthAndResetZoom,
    FullScreen,
    GoDown,
    GoLeft,
    GoRight,
    GoToParentFolder,
    GoUp,
    HidePreview,
    OpenFolder,
    OpenImageInNewTab,
    OpenPreferences,
    ShowPreview,
    ToggleFileManager,
    ToggleFileManagerVisibility,
    ToggleGrid,
    ZAutoFit,
    ZFill,
    ZResetMatrix,
    ZToggleStretchMode,
    ZUniform,
}

public class HotKeyManager
{
    #region Private Fields

    private readonly Dictionary<KeyGesture, KeyCommand> _defaultKeymap = new()
    {
        //App
        {new KeyGesture(Key.F11), KeyCommand.FullScreen},
        {new KeyGesture(Key.H), KeyCommand.GoLeft},
        {new KeyGesture(Key.Left), KeyCommand.GoLeft},
        {new KeyGesture(Key.PageUp), KeyCommand.GoLeft},
        {new KeyGesture(Key.L), KeyCommand.GoRight},
        {new KeyGesture(Key.Right), KeyCommand.GoRight},
        {new KeyGesture(Key.PageDown), KeyCommand.GoRight},
        //Tabs
        {new KeyGesture(Key.A), KeyCommand.ZAutoFit},
        // {new KeyGesture(Key.F), KeyCommand.ZFill},
        {new KeyGesture(Key.G), KeyCommand.ToggleGrid},
        {new KeyGesture(Key.H, KeyModifiers.Control), KeyCommand.FitToHeightAndResetZoom},
        {new KeyGesture(Key.H, KeyModifiers.Shift), KeyCommand.FitToHeightAndResetZoom},
        {new KeyGesture(Key.OemComma, KeyModifiers.Control), KeyCommand.OpenPreferences},
        {new KeyGesture(Key.P, KeyModifiers.Control), KeyCommand.OpenPreferences},
        {new KeyGesture(Key.R), KeyCommand.ZResetMatrix},
        {new KeyGesture(Key.S), KeyCommand.ZToggleStretchMode},
        {new KeyGesture(Key.U), KeyCommand.ZUniform},
        {new KeyGesture(Key.W, KeyModifiers.Control), KeyCommand.FitToWidthAndResetZoom},
        {new KeyGesture(Key.W, KeyModifiers.Shift), KeyCommand.FitToWidthAndResetZoom},
        //File Manager
        {new KeyGesture(Key.K), KeyCommand.GoUp},
        {new KeyGesture(Key.Up), KeyCommand.GoUp},
        {new KeyGesture(Key.J), KeyCommand.GoDown},
        {new KeyGesture(Key.Down), KeyCommand.GoDown},
        {new KeyGesture(Key.Back), KeyCommand.GoToParentFolder},
        {new KeyGesture(Key.Enter), KeyCommand.OpenFolder},
        {new KeyGesture(Key.F), KeyCommand.ToggleFileManager},
        {new KeyGesture(Key.F, KeyModifiers.Shift), KeyCommand.ToggleFileManagerVisibility},
        {new KeyGesture(Key.O), KeyCommand.OpenImageInNewTab},
        {new KeyGesture(Key.Space), KeyCommand.OpenImageInNewTab},
        //File Preview
        {new KeyGesture(Key.End), KeyCommand.LastImage},
        {new KeyGesture(Key.Escape), KeyCommand.HidePreview},
        {new KeyGesture(Key.Home), KeyCommand.FirstImage},
        {new KeyGesture(Key.P), KeyCommand.ShowPreview},
    };

    private static string HotKeyPath => Path.Combine(Settings.SettingsPath, "Hotkeys.json");

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        // IgnoreReadOnlyFields = true, 
        // IgnoreReadOnlyProperties = true,
        Converters =
            {new KeyGestureConverter(), new KeyCommandConverter(), new DictionaryKeyGestureKeyCommandConverter(),}
    };

    #endregion

    #region Private Methods

    private void SetBinding(KeyCommand action, KeyGesture newBinding, KeyGesture? oldBinding = null)
    {
        Log.Debug("HotKeyManager: SetBinding, {Action}, {NewBinding}, {OldBinding}", action, newBinding, oldBinding);
        if (oldBinding is not null)
        {
            UserKeymap.Remove(oldBinding);
        }

        UserKeymap[newBinding] = action;
    }

    private Dictionary<KeyGesture, KeyCommand>? LoadUserKeymap()
    {
        Log.Debug("HotKeyManager: LoadUserKeymap, Loading user keymap from {HotKeyPath}", HotKeyPath);
        if (!File.Exists(HotKeyPath))
            return null;

        var json = File.ReadAllText(HotKeyPath);
        var dictionary = JsonSerializer.Deserialize<Dictionary<KeyGesture, KeyCommand>>(json, JsonSerializerOptions);
        return dictionary is {Count: 0} ? _defaultKeymap : dictionary;
    }

    #endregion

    #region CTOR

    public HotKeyManager()
    {
        Log.Debug("Loading user keymap...");
        UserKeymap = LoadUserKeymap() ?? new Dictionary<KeyGesture, KeyCommand>(_defaultKeymap);
    }

    #endregion

    #region Public Properties

    public readonly Dictionary<KeyGesture, KeyCommand> UserKeymap;

    #endregion

    #region Public Methods

    public KeyCommand GetBinding(KeyGesture action)
    {
        return UserKeymap.GetValueOrDefault(action, KeyCommand.None);
    }


    public bool TrySetBinding(KeyCommand action, KeyGesture keyGesture, out KeyCommand oldCommand,
        KeyGesture? oldKeyGesture = null, bool overwrite = false)
    {
        Log.Debug("HotKeyManager: TrySetBinding, {Action}, {KeyGesture}, {OldBinding}", action, keyGesture,
            oldKeyGesture);
        oldCommand = KeyCommand.None;
        // Overwrite path
        if (overwrite)
        {
            SetBinding(action, keyGesture, oldKeyGesture);
            return true;
        }

        // Bad path
        if (UserKeymap.TryGetValue(keyGesture, out var value))
        {
            oldCommand = value;
            Log.Debug("HotKeyManager: TrySetBinding, {KeyGesture} already bound to {OldCommand}", keyGesture,
                oldCommand);
            return false;
        }

        // Good path
        SetBinding(action, keyGesture, oldKeyGesture);
        return true;
    }

    public void SaveUserKeymap()
    {
        Log.Debug("HotKeyManager: SaveUserKeymap, Saving user keymap to {HotKeyPath}", HotKeyPath);
        var jsonKeymap = JsonSerializer.Serialize(UserKeymap, JsonSerializerOptions);
        File.WriteAllText(HotKeyPath, jsonKeymap);
    }

    #endregion
}