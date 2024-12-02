using System;
using Marin.UI.Models;
using ReactiveUI;

namespace Marin.UI.ViewModels.Tabs.Preferences;

public class StatusBarPreferencesViewModel : ViewModelBase
{
    private bool _isVisible;
    private bool _autoHideStatusBar;
    private int _autoHideStatusBarDelay;

    public StatusBarPreferencesViewModel()
    {
        AutoHideStatusBar = Settings.Instance.StatusBarPreference.AutoHideStatusBar;
        AutoHideStatusBarDelay = Settings.Instance.StatusBarPreference.AutoHideStatusBarDelay;
        this.WhenAnyValue(x => x.AutoHideStatusBar)
            .Subscribe(value => Settings.Instance.StatusBarPreference.AutoHideStatusBar = value);
        this.WhenAnyValue(x => x.AutoHideStatusBarDelay)
            .Subscribe(value => Settings.Instance.StatusBarPreference.AutoHideStatusBarDelay = value);
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }

    public bool AutoHideStatusBar
    {
        get => _autoHideStatusBar;
        set => this.RaiseAndSetIfChanged(ref _autoHideStatusBar, value);
    }

    public int AutoHideStatusBarDelay
    {
        get => _autoHideStatusBarDelay;
        set => this.RaiseAndSetIfChanged(ref _autoHideStatusBarDelay, value);
    }
}