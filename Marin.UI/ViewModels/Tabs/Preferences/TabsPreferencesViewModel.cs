using System;
using Marin.UI.Models;
using ReactiveUI;

namespace Marin.UI.ViewModels.Tabs.Preferences;

public class TabsPreferencesViewModel : ViewModelBase
{
    private bool _isVisible;
    private bool _isDuplicateTabsAllowed;
    private bool _isSettingsTabCycled;
    private bool _isTabSettingsClosable;
    private bool _shouldImageLoop;
    

    public TabsPreferencesViewModel()
    {
        IsDuplicateTabsAllowed = Settings.Instance.TabsPreference.IsDuplicateTabsAllowed;
        IsSettingsTabCycled = Settings.Instance.TabsPreference.IsSettingsTabCycled;
        IsTabSettingsClosable = Settings.Instance.TabsPreference.IsTabSettingsClosable;
        ShouldImageLoop = Settings.Instance.TabsPreference.Loop;
        
        this.WhenAnyValue(x => x.IsDuplicateTabsAllowed)
            .Subscribe(value => Settings.Instance.TabsPreference.IsDuplicateTabsAllowed = value);
        this.WhenAnyValue(x => x.IsSettingsTabCycled)
            .Subscribe(value => Settings.Instance.TabsPreference.IsSettingsTabCycled = value);
        this.WhenAnyValue(x => x.IsTabSettingsClosable)
            .Subscribe(value => Settings.Instance.TabsPreference.IsTabSettingsClosable = value);

        this.WhenAnyValue(x => x.ShouldImageLoop).Subscribe(value => Settings.Instance.TabsPreference.Loop = value);
       
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }

    public bool IsDuplicateTabsAllowed
    {
        get => _isDuplicateTabsAllowed;
        set => this.RaiseAndSetIfChanged(ref _isDuplicateTabsAllowed, value);
    }

    public bool IsSettingsTabCycled
    {
        get => _isSettingsTabCycled;
        set => this.RaiseAndSetIfChanged(ref _isSettingsTabCycled, value);
    }

    public bool ShouldImageLoop
    {
        get => _shouldImageLoop;
        set => this.RaiseAndSetIfChanged(ref _shouldImageLoop, value);
    }

    public bool IsTabSettingsClosable
    {
        get => _isTabSettingsClosable;
        set => this.RaiseAndSetIfChanged(ref _isTabSettingsClosable, value);
    }

   
}