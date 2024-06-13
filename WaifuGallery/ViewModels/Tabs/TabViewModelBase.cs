using System;
using ReactiveUI;

namespace WaifuGallery.ViewModels.Tabs;

public class TabViewModelBase : ViewModelBase
{
    private string _header = string.Empty;

    public Guid Id { get; protected init; }

    public string Header
    {
        get => _header;
        set => this.RaiseAndSetIfChanged(ref _header, value);
    }
}