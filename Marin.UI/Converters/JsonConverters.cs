﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Input;
using Marin.UI.Models;

namespace Marin.UI.Converters;

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
            writer.WritePropertyName(ConvertKeyCommandToString(kvp.Value)); //KeyCommand
            JsonSerializer.Serialize(writer, kvp.Key, options); //KeyGesture
        }

        writer.WriteEndObject();
    }

    private static string ConvertKeyCommandToString(KeyCommand keyCommand)
    {
        return keyCommand switch
        {
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
            KeyCommand.ToggleGrid => "ToggleGrid",
            KeyCommand.ZAutoFit => "ZAutoFit",
            KeyCommand.ZFill => "ZFill",
            KeyCommand.ZResetMatrix => "ZResetMatrix",
            KeyCommand.ZToggleStretchMode => "ZToggleStretchMode",
            KeyCommand.ZUniform => "ZUniform",
            _ => throw new ArgumentOutOfRangeException(nameof(keyCommand), keyCommand, null)
        };
    }
}