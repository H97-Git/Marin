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

public class ShortcutViewModel : ViewModelBase
{
    private KeyCommand _keyCommand = KeyCommand.None;

    public ShortcutViewModel(KeyCommand command)
    {
        KeyCommand = command;
    }

    public ObservableCollection<KeyGesture> Gestures { get; set; } = [];

    public KeyCommand KeyCommand
    {
        get => _keyCommand;
        set => this.RaiseAndSetIfChanged(ref _keyCommand, value);
    }

    public ICommand UpdateKeyCommand => ReactiveCommand.Create<KeyGesture>(UpdateKey);

    private async void UpdateKey(KeyGesture oldKeyGesture)
    {
        var keyGesture = await ShowSetKeyDialogAsync();
        if (keyGesture is null) return;
        if (!Gestures.Remove(oldKeyGesture)) return;
        if (Settings.Instance.HotKeyManager.TrySetBinding(KeyCommand, keyGesture, oldKeyGesture, out var oldBinding))
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