using ReactiveUI;

namespace WaifuGallery.ViewModels.Tabs;

public enum TabType
{
    Unknown,
    Image,
    Preferences,
}

public class TabViewModelBase : ViewModelBase
{
    private string _header = string.Empty;

    public string Id { get; protected init; } = string.Empty;

    public TabType TabType { get; protected init; } = TabType.Unknown;

    public string Header
    {
        get => _header;
        set => this.RaiseAndSetIfChanged(ref _header, value);
    }
}