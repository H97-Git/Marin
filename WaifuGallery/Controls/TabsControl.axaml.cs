using System;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using WaifuGallery.ViewModels.Tabs;

namespace WaifuGallery.Controls;

public partial class TabsControl : UserControl
{
    private TabsViewModel? TabsViewModel => DataContext as TabsViewModel;

    public TabsControl()
    {
        InitializeComponent();
        ImagesTabControl.SelectionChanged += ImagesTabControl_OnSelectionChanged;
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        if (TabsViewModel is null) return;
        TabsViewModel.ControlSize = TabsUserControl.Bounds.Size;
    }

    private void ImagesTabControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) =>
        TabsViewModel?.SelectionChanged(e);
}