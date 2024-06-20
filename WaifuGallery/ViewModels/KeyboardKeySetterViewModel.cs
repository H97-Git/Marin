using System;
using Avalonia.Input;
using ReactiveUI;

namespace WaifuGallery.ViewModels;

public class KeyboardKeySetterViewModel : ViewModelBase
{
    private Key _key = Key.None;
    private string _keyName = string.Empty;

    public KeyboardKeySetterViewModel()
    {
        this.WhenAnyValue(x => x.Key).Subscribe(x =>
        {
            if (x == Key.None) return;
            KeyName = Key switch
            {
                Key.D0 => "0",
                Key.D1 => "1",
                Key.D2 => "2",
                Key.D3 => "3",
                Key.D4 => "4",
                Key.D5 => "5",
                Key.D6 => "6",
                Key.D7 => "7",
                Key.D8 => "8",
                Key.D9 => "9",
                Key.LWin => "Win",
                Key.RWin => "RWin",
                Key.LaunchApplication1 => "App1",
                Key.LaunchApplication2 => "App2",
                Key.OemSemicolon => ";",
                Key.OemPlus => "+",
                Key.OemComma => ",",
                Key.OemMinus => "-",
                Key.OemPeriod => ".",
                Key.OemQuestion => "/",
                Key.OemTilde => "`",
                Key.AbntC1 => "AbntC1",
                Key.AbntC2 => "AbntC2",
                Key.OemOpenBrackets => "[",
                Key.OemPipe => "\\",
                Key.OemCloseBrackets => "]",
                Key.OemQuotes => "\"",
                Key.Oem8 => "Oem8",
                Key.OemBackslash => "\\",
                Key.ImeProcessed => "ImeProcessed",
                Key.OemAttn => "Attn",
                Key.OemFinish => "Finish",
                Key.DbeHiragana => "Hiragana",
                Key.DbeSbcsChar => "SbcsChar",
                Key.DbeDbcsChar => "DbcsChar",
                Key.OemBackTab => "BackTab",
                Key.DbeNoRoman => "NoRoman",
                Key.CrSel => "CrSel",
                Key.ExSel => "ExSel",
                Key.EraseEof => "EraseEof",
                Key.DbeNoCodeInput => "NoCodeInput",
                Key.NoName => "NoName",
                Key.DbeEnterDialogConversionMode => "EnterDialogConversionMode",
                Key.OemClear => "Clear",
                Key.DeadCharProcessed => "DeadCharProcessed",
                _ => Key.ToString()
            };
        });
    }

    public Key Key
    {
        get => _key;
        set => this.RaiseAndSetIfChanged(ref _key, value);
    }

    public string KeyName
    {
        get => _keyName;
        set => this.RaiseAndSetIfChanged(ref _keyName, value);
    }
}