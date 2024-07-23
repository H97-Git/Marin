using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Input;
using WaifuGallery.Models;

namespace WaifuGallery.Helpers;

public class KeyGestureConverter : JsonConverter<KeyGesture>
{
    public override KeyGesture Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return KeyGesture.Parse(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, KeyGesture value, JsonSerializerOptions options)
    {
        var str = value.ToString();
        writer.WriteStringValue(str);
    }
}

public class KeyCommandConverter : JsonConverter<KeyCommand>
{
    public override KeyCommand Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return (KeyCommand) Enum.Parse(typeof(KeyCommand), reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, KeyCommand value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(nameof(value));
    }
}

public class DictionaryKeyGestureKeyCommandConverter : JsonConverter<Dictionary<KeyGesture, KeyCommand>>
{
    public override Dictionary<KeyGesture, KeyCommand> Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var result = new Dictionary<KeyGesture, KeyCommand>();

        var jsonObject = JsonDocument.ParseValue(ref reader).RootElement;
        foreach (var property in jsonObject.EnumerateObject())
        {
            // var keyGesture = KeyGesture.Parse(property.Name);
            // var keyCommand = JsonSerializer.Deserialize<KeyCommand>(property.Value.GetRawText(), options);
            // result[keyGesture] = keyCommand;
            var keyGesture = KeyGesture.Parse(property.Value.GetString()!);
            var keyCommand = (KeyCommand) Enum.Parse(typeof(KeyCommand), property.Name!);
            result[keyGesture] = keyCommand;
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<KeyGesture, KeyCommand> value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var kvp in value)
        {
            // writer.WritePropertyName(kvp.Key.ToString());
            // JsonSerializer.Serialize(writer, kvp.Value, options);
            writer.WritePropertyName(ConvertKeyCommandToString(kvp.Value)); //KeyCommand
            JsonSerializer.Serialize(writer, kvp.Key, options); //KeyGesture
        }

        writer.WriteEndObject();
    }

    private static string ConvertKeyCommandToString(KeyCommand keyCommand)
    {
        return keyCommand switch
        {
            // KeyCommand.None => nameof(KeyCommand.None),
            // KeyCommand.FirstImage => nameof(KeyCommand.FirstImage),
            // KeyCommand.FitToHeightAndResetZoom => nameof(KeyCommand.FitToHeightAndResetZoom),
            // KeyCommand.FitToWidthAndResetZoom => nameof(KeyCommand.FitToWidthAndResetZoom),
            // KeyCommand.FullScreen => nameof(KeyCommand.FullScreen),
            // KeyCommand.GoDown => nameof(KeyCommand.GoDown),
            // KeyCommand.GoLeft => nameof(KeyCommand.GoLeft),
            // KeyCommand.GoRight => nameof(KeyCommand.GoRight),
            // KeyCommand.GoToParentFolder => nameof(KeyCommand.GoToParentFolder),
            // KeyCommand.GoUp => nameof(KeyCommand.GoUp),
            // KeyCommand.HidePreview => nameof(KeyCommand.HidePreview),
            // KeyCommand.LastImage => nameof(KeyCommand.LastImage),
            // KeyCommand.NextImage => nameof(KeyCommand.NextImage),
            // KeyCommand.OpenFolder => nameof(KeyCommand.OpenFolder),
            // KeyCommand.OpenImageInNewTab => nameof(KeyCommand.OpenImageInNewTab),
            // KeyCommand.OpenPreferences => nameof(KeyCommand.OpenPreferences),
            // KeyCommand.PreviousImage => nameof(KeyCommand.PreviousImage),
            // KeyCommand.ShowPreview => nameof(KeyCommand.ShowPreview),
            // KeyCommand.ToggleFileExplorer => nameof(KeyCommand.ToggleFileExplorer),
            // KeyCommand.ToggleFileExplorerVisibility => nameof(KeyCommand.ToggleFileExplorerVisibility),
            // KeyCommand.ZAutoFit => nameof(KeyCommand.ZAutoFit),
            // KeyCommand.ZFill => nameof(KeyCommand.ZFill),
            // KeyCommand.ZResetMatrix => nameof(KeyCommand.ZResetMatrix),
            // KeyCommand.ZToggleStretchMode => nameof(KeyCommand.ZToggleStretchMode),
            // KeyCommand.ZUniform => nameof(KeyCommand.ZUniform),
            KeyCommand.None => "None",
            KeyCommand.FirstImage => "FirstImage",
            KeyCommand.FitToHeightAndResetZoom => "FitToHeightAndResetZoom",
            KeyCommand.FitToWidthAndResetZoom => "FitToWidthAndResetZoom",
            KeyCommand.FullScreen => "FullScreen",
            KeyCommand.GoDown => "GoDown",
            KeyCommand.GoLeft => "GoLeft",
            KeyCommand.GoRight => "GoRight",
            KeyCommand.GoToParentFolder => "GoToParentFolder",
            KeyCommand.GoUp => "GoUp",
            KeyCommand.HidePreview => "HidePreview",
            KeyCommand.LastImage => "LastImage",
            KeyCommand.OpenFolder => "OpenFolder",
            KeyCommand.OpenImageInNewTab => "OpenImageInNewTab",
            KeyCommand.OpenPreferences => "OpenPreferences",
            KeyCommand.ShowPreview => "ShowPreview",
            KeyCommand.ToggleFileManager => "ToggleFileManager",
            KeyCommand.ToggleFileManagerVisibility => "ToggleFileManagerVisibility",
            KeyCommand.ZAutoFit => "ZAutoFit",
            KeyCommand.ZFill => "ZFill",
            KeyCommand.ZResetMatrix => "ZResetMatrix",
            KeyCommand.ZToggleStretchMode => "ZToggleStretchMode",
            KeyCommand.ZUniform => "ZUniform",
            _ => throw new ArgumentOutOfRangeException(nameof(keyCommand), keyCommand, null)
        };
    }
}