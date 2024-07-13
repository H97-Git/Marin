using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Input;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using WaifuGallery.Commands;
using WaifuGallery.Controls.Dialogs;
using WaifuGallery.Models;
using WaifuGallery.ViewModels.Dialogs;

namespace WaifuGallery.ViewModels;

public class ShortcutViewModel(KeyCommand keyCommand) : ViewModelBase
{
    public ObservableCollection<KeyGesture> Gestures { get; set; } = [];

    public string KeyCommandString
        => keyCommand switch
        {
            KeyCommand.None => "None",
            KeyCommand.FirstImage => "First Image",
            KeyCommand.LastImage => "Last Image",
            KeyCommand.NextImage => "Next Image",
            KeyCommand.PreviousImage => "Previous Image",
            KeyCommand.GoUp => "Go Up",
            KeyCommand.GoDown => "Go Down",
            KeyCommand.GoLeft => "Go Left",
            KeyCommand.GoRight => "Go Right",
            KeyCommand.GoToParentFolder => "Go To Parent Folder",
            KeyCommand.OpenFolder => "Open Folder",
            KeyCommand.OpenImageInNewTab => "Open Image In New Tab",
            KeyCommand.ToggleFileManager => "Toggle File Manager",
            KeyCommand.ToggleFileManagerVisibility => "Toggle File Manager Visibility",
            KeyCommand.ShowPreview => "Show Preview",
            KeyCommand.HidePreview => "Hide Preview",
            KeyCommand.FullScreen => "Full Screen",
            KeyCommand.FitToWidthAndResetZoom => "Fit To Width And Reset Zoom",
            KeyCommand.FitToHeightAndResetZoom => "Fit To Height And Reset Zoom",
            KeyCommand.OpenPreferences => "Open Preferences",
            KeyCommand.ZAutoFit => "Auto Fit",
            KeyCommand.ZFill => "Fill",
            KeyCommand.ZResetMatrix => "Reset Matrix",
            KeyCommand.ZToggleStretchMode => "Toggle Stretch Mode",
            KeyCommand.ZUniform => "Uniform",
            _ => throw new ArgumentOutOfRangeException(nameof(keyCommand), keyCommand, null)
        };


    public ICommand UpdateKeyCommand => ReactiveCommand.Create<KeyGesture>(UpdateKey);

    private async void UpdateKey(KeyGesture oldKeyGesture)
    {
        var keyGesture = await ShowSetKeyDialogAsync();
        if (keyGesture is null) return;
        if (!Gestures.Remove(oldKeyGesture)) return;
        if (Settings.Instance.HotKeyManager.TrySetBinding(keyCommand, keyGesture, oldKeyGesture, out var oldBinding))
        {
            Gestures.Add(keyGesture);
        }
        else
        {
            // Show another dialog if the binding is already in use
            Gestures.Add(oldKeyGesture);
            MessageBus.Current.SendMessage(new SendMessageToStatusBarCommand(InfoBarSeverity.Warning,
                $"{keyGesture} is already used by {oldBinding}"));
        }
    }

    private async Task<KeyGesture?> ShowSetKeyDialogAsync()
    {
        var dialog = new ContentDialog()
        {
            Title = "Press any key",
            PrimaryButtonText = "Ok",
            SecondaryButtonText = "Cancel",
        };
        var keySetterViewModel = new KeyboardKeySetterViewModel();
        dialog.Content = new KeyboardKeySetter()
        {
            DataContext = keySetterViewModel,
            OnEscapePressed = (_, _) => { dialog.Hide(); }
        };
        var dialogResult = await dialog.ShowAsync();
        return dialogResult is ContentDialogResult.Secondary ? null : keySetterViewModel.KeyGesture;
    }
}