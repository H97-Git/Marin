using System;
using ReactiveUI;
using WaifuGallery.Models;

namespace WaifuGallery.ViewModels.Tabs.Preferences;

public class TabsPreferencesViewModel : ViewModelBase
{
    private bool _isVisible;
    private bool _isDuplicateTabsAllowed;
    private bool _isSettingsTabCycled;
    private bool _isTabSettingsClosable;
    private bool _shouldImageLoop;
    private bool _shouldLoadLastSessionOnStartUp;
    private bool _shouldOpenPreferencesOnStartUp;
    private bool _shouldSaveLastSessionOnExit;

    public TabsPreferencesViewModel()
    {
        IsDuplicateTabsAllowed = Settings.Instance.TabsPreference.IsDuplicateTabsAllowed;
        IsSettingsTabCycled = Settings.Instance.TabsPreference.IsSettingsTabCycled;
        IsTabSettingsClosable = Settings.Instance.TabsPreference.IsTabSettingsClosable;
        ShouldImageLoop = Settings.Instance.TabsPreference.Loop;
        ShouldOpenPreferencesOnStartUp = Settings.Instance.TabsPreference.OpenPreferencesOnStartup;
        ShouldLoadLastSessionOnStartUp = Settings.Instance.TabsPreference.LoadLastSessionOnStartUp;
        ShouldSaveLastSessionOnExit = Settings.Instance.TabsPreference.SaveLastSessionOnExit;
        this.WhenAnyValue(x => x.IsDuplicateTabsAllowed)
            .Subscribe(value => Settings.Instance.TabsPreference.IsDuplicateTabsAllowed = value);
        this.WhenAnyValue(x => x.IsSettingsTabCycled)
            .Subscribe(value => Settings.Instance.TabsPreference.IsSettingsTabCycled = value);
        this.WhenAnyValue(x => x.IsTabSettingsClosable)
            .Subscribe(value => Settings.Instance.TabsPreference.IsTabSettingsClosable = value);

        this.WhenAnyValue(x => x.ShouldImageLoop).Subscribe(value => Settings.Instance.TabsPreference.Loop = value);
        this.WhenAnyValue(x => x.ShouldOpenPreferencesOnStartUp).Subscribe(value =>
            Settings.Instance.TabsPreference.OpenPreferencesOnStartup = value);
        this.WhenAnyValue(x => x.ShouldSaveLastSessionOnExit)
            .Subscribe(value => Settings.Instance.TabsPreference.SaveLastSessionOnExit = value);
        this.WhenAnyValue(x => x.ShouldLoadLastSessionOnStartUp)
            .Subscribe(value => Settings.Instance.TabsPreference.LoadLastSessionOnStartUp = value);
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

    public bool ShouldOpenPreferencesOnStartUp
    {
        get => _shouldOpenPreferencesOnStartUp;
        set => this.RaiseAndSetIfChanged(ref _shouldOpenPreferencesOnStartUp, value);
    }

    public bool ShouldSaveLastSessionOnExit
    {
        get => _shouldSaveLastSessionOnExit;
        set => this.RaiseAndSetIfChanged(ref _shouldSaveLastSessionOnExit, value);
    }

    public bool ShouldLoadLastSessionOnStartUp
    {
        get => _shouldLoadLastSessionOnStartUp;
        set => this.RaiseAndSetIfChanged(ref _shouldLoadLastSessionOnStartUp, value);
    }
}