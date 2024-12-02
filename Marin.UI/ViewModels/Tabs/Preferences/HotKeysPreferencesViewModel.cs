using System.Collections.ObjectModel;
using System.Linq;
using Marin.UI.Models;
using ReactiveUI;

namespace Marin.UI.ViewModels.Tabs.Preferences;

public class HotKeysPreferencesViewModel : ViewModelBase
{
    private bool _isVisible;

    public HotKeysPreferencesViewModel()
    {
        var groups = Settings.Instance.HotKeyManager.UserKeymap.GroupBy(x => x.Value);
        foreach (var group in groups)
        {
            var shortcut = new ShortcutViewModel(group.Key);
            foreach (var keyGestureKeyCommand in group)
            {
                shortcut.Gestures.Add(keyGestureKeyCommand.Key);
            }

            Shortcuts.Add(shortcut);
        }

        Shortcuts = new ObservableCollection<ShortcutViewModel>(Shortcuts.OrderBy(x => x.KeyCommandText));
    }

    public ObservableCollection<ShortcutViewModel> Shortcuts { get; init; } = [];

    public bool IsVisible
    {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }
}