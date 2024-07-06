using System;
using Avalonia.Input;
using ReactiveUI;

namespace WaifuGallery.ViewModels.Dialogs;

public class KeyboardKeySetterViewModel : ViewModelBase
{
    #region Private Fields

    private KeyEventArgs? _keyEventArgs;
    private string _keyName = string.Empty;

    #endregion

    #region CTOR

    public KeyboardKeySetterViewModel()
    {
        this.WhenAnyValue(x => x.KeyEventArgs).Subscribe(x =>
        {
            if (x is null || x.Key is Key.Escape) return;
            var key = x.Key switch
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
                Key.LeftAlt => "",
                Key.RightAlt => "",
                Key.LeftCtrl => "",
                Key.RightCtrl => "",
                Key.LeftShift => "",
                Key.RightShift => "",
                _ => x.Key.ToString()
            };
            var modifier = x.KeyModifiers switch
            {
                KeyModifiers.Control => "Ctrl",
                KeyModifiers.Shift => "Shift",
                KeyModifiers.Alt => "Alt",
                _ => string.Empty,
            };
            KeyName = KeyEventArgs?.KeyModifiers is not KeyModifiers.None ? $"{modifier}+{key}" : key;
        });
    }

    #endregion

    #region Public Properties

    public KeyEventArgs? KeyEventArgs
    {
        get => _keyEventArgs;
        set => this.RaiseAndSetIfChanged(ref _keyEventArgs, value);
    }

    public string KeyName
    {
        get => _keyName;
        set => this.RaiseAndSetIfChanged(ref _keyName, value);
    }

    #endregion
}