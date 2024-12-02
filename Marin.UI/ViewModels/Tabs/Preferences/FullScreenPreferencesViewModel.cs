using System;
using Marin.UI.Models;
using ReactiveUI;

namespace Marin.UI.ViewModels.Tabs.Preferences;

public class FullScreenPreferencesViewModel : ViewModelBase
{
    private bool _isVisible;
    private bool _shouldHideFileManager;
    private bool _shouldHideMenuBar;
    private bool _shouldHideStatusBar;
    private bool _shouldHideTabsHeader;

    public FullScreenPreferencesViewModel()
    {
        ShouldHideFileManager = Settings.Instance.FileManagerPreference.ShouldHideFileManager;
        ShouldHideMenuBar = Settings.Instance.ShouldHideMenuBar;
        ShouldHideStatusBar = Settings.Instance.StatusBarPreference.ShouldHideStatusBar;
        ShouldHideTabsHeader = Settings.Instance.TabsPreference.ShouldHideTabsHeader;
        this.WhenAnyValue(x => x.ShouldHideFileManager)
            .Subscribe(value => Settings.Instance.FileManagerPreference.ShouldHideFileManager = value);
        this.WhenAnyValue(x => x.ShouldHideMenuBar)
            .Subscribe(value => Settings.Instance.ShouldHideMenuBar = value);
        this.WhenAnyValue(x => x.ShouldHideStatusBar)
            .Subscribe(value => Settings.Instance.StatusBarPreference.ShouldHideStatusBar = value);
        this.WhenAnyValue(x => x.ShouldHideTabsHeader)
            .Subscribe(value => Settings.Instance.TabsPreference.ShouldHideTabsHeader = value);
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }

    public bool ShouldHideFileManager
    {
        get => _shouldHideFileManager;
        set => this.RaiseAndSetIfChanged(ref _shouldHideFileManager, value);
    }

    public bool ShouldHideStatusBar
    {
        get => _shouldHideStatusBar;
        set => this.RaiseAndSetIfChanged(ref _shouldHideStatusBar, value);
    }

    public bool ShouldHideMenuBar
    {
        get => _shouldHideMenuBar;
        set => this.RaiseAndSetIfChanged(ref _shouldHideMenuBar, value);
    }

    public bool ShouldHideTabsHeader
    {
        get => _shouldHideTabsHeader;
        set => this.RaiseAndSetIfChanged(ref _shouldHideTabsHeader, value);
    }
}