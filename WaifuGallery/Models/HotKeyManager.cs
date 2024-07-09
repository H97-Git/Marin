using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Input;
using WaifuGallery.Helpers;

namespace WaifuGallery.Models;

public enum KeyCommand
{
    None,
    FirstImage,
    LastImage,
    NextImage,
    PreviousImage,
    GoUp,
    GoDown,
    GoLeft,
    GoRight,
    GoToParentFolder,
    OpenFolder,
    OpenImageInNewTab,
    ToggleFileExplorer,
    ToggleFileExplorerVisibility,
    ShowPreview,
    HidePreview,
    FullScreen,
    FitToWidthAndResetZoom,
    FitToHeightAndResetZoom,
    OpenPreferences,
    ZAutoFit,
    ZFill,
    ZResetMatrix,
    ZToggleStretchMode,
    ZUniform,
}

public class HotKeyManager
{
    private readonly Dictionary<KeyGesture, KeyCommand> _defaultKeymap = new()
    {
        //App
        {new KeyGesture(Key.F11), KeyCommand.FullScreen},
        {new KeyGesture(Key.K), KeyCommand.GoUp},
        {new KeyGesture(Key.Up), KeyCommand.GoUp},
        {new KeyGesture(Key.J), KeyCommand.GoDown},
        {new KeyGesture(Key.Down), KeyCommand.GoDown},
        {new KeyGesture(Key.H), KeyCommand.GoLeft},
        {new KeyGesture(Key.Left), KeyCommand.GoLeft},
        {new KeyGesture(Key.PageUp), KeyCommand.PreviousImage},
        {new KeyGesture(Key.L), KeyCommand.GoRight},
        {new KeyGesture(Key.Right), KeyCommand.GoRight},
        {new KeyGesture(Key.PageDown), KeyCommand.NextImage},
        //Tabs
        {new KeyGesture(Key.A), KeyCommand.ZAutoFit},
        // {new KeyGesture(Key.F), Hk.ZFill},
        {new KeyGesture(Key.H, KeyModifiers.Control), KeyCommand.FitToHeightAndResetZoom},
        {new KeyGesture(Key.H, KeyModifiers.Shift), KeyCommand.FitToHeightAndResetZoom},
        {new KeyGesture(Key.OemComma, KeyModifiers.Control), KeyCommand.OpenPreferences},
        {new KeyGesture(Key.P, KeyModifiers.Control), KeyCommand.OpenPreferences},
        {new KeyGesture(Key.R), KeyCommand.ZResetMatrix},
        {new KeyGesture(Key.S), KeyCommand.ZToggleStretchMode},
        {new KeyGesture(Key.U), KeyCommand.ZUniform},
        {new KeyGesture(Key.W, KeyModifiers.Control), KeyCommand.FitToWidthAndResetZoom},
        {new KeyGesture(Key.W, KeyModifiers.Shift), KeyCommand.FitToWidthAndResetZoom},
        //File Explorer
        {new KeyGesture(Key.Back), KeyCommand.GoToParentFolder},
        {new KeyGesture(Key.Enter), KeyCommand.OpenFolder},
        {new KeyGesture(Key.F), KeyCommand.ToggleFileExplorer},
        {new KeyGesture(Key.F, KeyModifiers.Shift), KeyCommand.ToggleFileExplorerVisibility},
        {new KeyGesture(Key.O), KeyCommand.OpenImageInNewTab},
        {new KeyGesture(Key.Space), KeyCommand.OpenImageInNewTab},
        //File Preview
        {new KeyGesture(Key.End), KeyCommand.LastImage},
        {new KeyGesture(Key.Escape), KeyCommand.HidePreview},
        {new KeyGesture(Key.Home), KeyCommand.FirstImage},
        {new KeyGesture(Key.P), KeyCommand.ShowPreview},
    };

    public readonly Dictionary<KeyGesture, KeyCommand> UserKeymap;

    private static string HotKeyPath => Path.Combine(Settings.SettingsPath, "Hotkeys.json");

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        // IgnoreReadOnlyFields = true, 
        // IgnoreReadOnlyProperties = true,
        Converters =
            {new KeyGestureConverter(), new KeyCommandConverter(), new DictionaryKeyGestureKeyCommandConverter(),}
    };

    public HotKeyManager()
    {
        UserKeymap = LoadUserKeymap() ?? new Dictionary<KeyGesture, KeyCommand>(_defaultKeymap);
    }

    public KeyCommand GetBinding(KeyGesture action)
    {
        return UserKeymap.GetValueOrDefault(action, KeyCommand.None);
        // : _defaultKeymap.TryGetValue(action, out binding)
        //     ? binding
    }

    public bool TrySetBinding(KeyCommand action, KeyGesture newBinding, KeyGesture oldBindingG,
        out KeyCommand oldBindingC, bool overwrite = false)
    {
        oldBindingC = KeyCommand.None;
        if (overwrite)
        {
            SetBinding(action, newBinding, oldBindingG);
            return true;
        }

        if (UserKeymap.TryGetValue(newBinding, out var value))
        {
            oldBindingC = value;
            return false;
        }

        SetBinding(action, newBinding, oldBindingG);
        return true;
    }

    private void SetBinding(KeyCommand action, KeyGesture newBinding, KeyGesture oldBinding)
    {
        UserKeymap.Remove(oldBinding);
        UserKeymap[newBinding] = action;
        // SaveUserKeymap();
    }

    private Dictionary<KeyGesture, KeyCommand>? LoadUserKeymap()
    {
        if (!File.Exists(HotKeyPath))
            return null;

        var json = File.ReadAllText(HotKeyPath);
        var dictionary = JsonSerializer.Deserialize<Dictionary<KeyGesture, KeyCommand>>(json, JsonSerializerOptions);
        return dictionary is {Count: 0} ? _defaultKeymap : dictionary;
    }

    public void SaveUserKeymap()
    {
        // Implement saving logic (e.g., to a JSON file)
        var jsonKeymap = JsonSerializer.Serialize(UserKeymap, JsonSerializerOptions);
        File.WriteAllText(HotKeyPath, jsonKeymap);
    }
}