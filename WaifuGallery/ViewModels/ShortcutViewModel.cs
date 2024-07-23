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
            KeyCommand.None => "none",
            KeyCommand.FirstImage => "First image",
            KeyCommand.LastImage => "Last image",
            KeyCommand.GoUp => "Go Up",
            KeyCommand.GoDown => "Go Down",
            KeyCommand.GoLeft => "Go Left / Load next image",
            KeyCommand.GoRight => "Go Right / Load next image",
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

    public string KeyCommandToolTipString => keyCommand switch
    {
        KeyCommand.None => "WTF?",
        KeyCommand.FirstImage => "Load the first image",
        KeyCommand.LastImage => "Load the last image",
        KeyCommand.GoUp => "Go up in file manager",
        KeyCommand.GoDown => "Go down in file manager",
        KeyCommand.GoLeft => "Go left in file manager / Load the previous image",
        KeyCommand.GoRight => "Go right in file manager / Load the next image",
        KeyCommand.GoToParentFolder => "Go to parent folder",
        KeyCommand.OpenFolder => "Open folder",
        KeyCommand.OpenImageInNewTab => "Open the selected image in a new tab",
        KeyCommand.ToggleFileManager => "Open or close the file manager",
        KeyCommand.ToggleFileManagerVisibility => "Toggle the visibility of the file manager",
        KeyCommand.ShowPreview => "Show the image preview",
        KeyCommand.HidePreview => "Hide the image preview",
        KeyCommand.FullScreen => "Toggle full screen mode",
        KeyCommand.FitToWidthAndResetZoom => "Fit the image to the width and reset the zoom",
        KeyCommand.FitToHeightAndResetZoom => "Fit the image to the height and reset the zoom",
        KeyCommand.OpenPreferences => "Open the preferences",
        KeyCommand.ZAutoFit => "Auto fit the image to the screen",
        KeyCommand.ZFill => "Fill the image to the screen",
        KeyCommand.ZResetMatrix => "Reset the image zoom matrix",
        KeyCommand.ZToggleStretchMode => "Toggle the image stretch mode",
        KeyCommand.ZUniform => "Toggle the image uniform scaling",
        _ => throw new ArgumentOutOfRangeException(nameof(keyCommand), keyCommand, null)
    };


    public ICommand UpdateKeyCommand => ReactiveCommand.Create<KeyGesture>(UpdateKey);
    public ICommand AddKeyCommand => ReactiveCommand.Create<KeyGesture>(_ => AddKey());

    private async void AddKey()
    {
        var keyGesture = await ShowSetKeyDialogAsync();
        if (keyGesture is null) return;
        if (Gestures.Contains(keyGesture)) return;
        if (Settings.Instance.HotKeyManager.TrySetBinding(keyCommand, keyGesture, out var oldBinding))
        {
            Gestures.Add(keyGesture);
        }
        else
        {
            MessageBus.Current.SendMessage(new SendMessageToStatusBarCommand(InfoBarSeverity.Warning,
                $"{keyGesture} is already used by {oldBinding}"));
        }
    }

    private async void UpdateKey(KeyGesture oldKeyGesture)
    {
        var keyGesture = await ShowSetKeyDialogAsync();
        if (keyGesture is null) return;
        if (!Gestures.Remove(oldKeyGesture)) return;
        if (Settings.Instance.HotKeyManager.TrySetBinding(keyCommand, keyGesture, out var oldBinding, oldKeyGesture))
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
        await dialog.ShowAsync();
        return keySetterViewModel.KeyGesture;
    }
}