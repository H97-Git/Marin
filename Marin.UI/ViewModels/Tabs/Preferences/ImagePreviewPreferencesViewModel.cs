using System;
using Marin.UI.Models;
using ReactiveUI;

namespace Marin.UI.ViewModels.Tabs.Preferences;

public class ImagePreviewPreferencesViewModel : ViewModelBase
{
    private bool _isVisible;
    private bool _previewFollowMouse;
    private bool _shouldImagePreviewLoop;
    private int _previewDefaultZoom;
    private int _previewDepth;

    public ImagePreviewPreferencesViewModel()
    {
        PreviewDefaultZoom = Settings.Instance.ImagePreviewPreference.DefaultZoom;
        PreviewDepth = Settings.Instance.ImagePreviewPreference.Depth;
        PreviewFollowMouse = Settings.Instance.ImagePreviewPreference.FollowMouse;
        ShouldImagePreviewLoop = Settings.Instance.ImagePreviewPreference.Loop;
        this.WhenAnyValue(x => x.PreviewDefaultZoom)
            .Subscribe(value => Settings.Instance.ImagePreviewPreference.DefaultZoom = value);
        this.WhenAnyValue(x => x.PreviewDepth)
            .Subscribe(value => Settings.Instance.ImagePreviewPreference.Depth = value);
        this.WhenAnyValue(x => x.PreviewFollowMouse)
            .Subscribe(value => Settings.Instance.ImagePreviewPreference.FollowMouse = value);
        this.WhenAnyValue(x => x.ShouldImagePreviewLoop)
            .Subscribe(value => Settings.Instance.ImagePreviewPreference.Loop = value);
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }

    public bool ShouldImagePreviewLoop
    {
        get => _shouldImagePreviewLoop;
        set => this.RaiseAndSetIfChanged(ref _shouldImagePreviewLoop, value);
    }

    public bool PreviewFollowMouse
    {
        get => _previewFollowMouse;
        set => this.RaiseAndSetIfChanged(ref _previewFollowMouse, value);
    }

    public int PreviewDepth
    {
        get => _previewDepth;
        set => this.RaiseAndSetIfChanged(ref _previewDepth, value);
    }

    public int PreviewDefaultZoom
    {
        get => _previewDefaultZoom;
        set => this.RaiseAndSetIfChanged(ref _previewDefaultZoom, value);
    }
}