using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using WaifuGallery.ViewModels;

namespace WaifuGallery.Controls;

public partial class TabsControl : UserControl
{
    private TabsViewModel? TabsViewModel => DataContext as TabsViewModel;

    public TabsControl()
    {
        InitializeComponent();
        ImagesTabControl.KeyDown += ImagesTabControl_OnKeyDown_SwitchTab;
        ImagesTabControl.SelectionChanged += ImagesTabControl_OnSelectionChanged;
        TabsUserControl.KeyDown += ImagesTabControl_OnKeyDown_SwitchTab;
    }

    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        // Find which tab is clicked
        var listBox = (ListBox) sender!;
        var index = listBox.SelectedIndex;
        if (index < 0) return;
        if (listBox.Items[index] is not ImageTabViewModel tab) return;
        // Set the content of the tab
        var image = new Bitmap(tab.CurrentImagePath);
        ImageTabContent.Source = image;
    }

    private void ImagesTabControl_OnKeyDown_SwitchTab(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Tab && (e.KeyModifiers & KeyModifiers.Control) == KeyModifiers.Control)
        {
            SwitchTab();
        }
    }


    private void ImagesTabControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var t = TabsUserControl.Bounds.Size.Height;
        var s = ImagesTabControl.Bounds.Size.Height;
        TabsViewModel?.FitToHeight(t);
    }

    private void TabHeader_OnPointerEntered_ToggleCloseButton(object? sender, PointerEventArgs e)
    {
        if (sender is not StackPanel stackPanel) return;
        if (stackPanel.DataContext is not ImageTabViewModel tabViewModel) return;
        tabViewModel.IsCloseButtonVisible = true;
    }

    private void TabHeader_OnPointerExited_ToggleCloseButton(object? sender, PointerEventArgs e)
    {
        if (sender is not StackPanel stackPanel) return;
        if (stackPanel.DataContext is not ImageTabViewModel tabViewModel) return;
        tabViewModel.IsCloseButtonVisible = false;
    }

    private void SwitchTab()
    {
        if (ImagesTabControl == null) return;
        var selectedIndex = ImagesTabControl.SelectedIndex;
        var itemCount = ImagesTabControl.Items.Count;

        // Calculate the index of the next tab
        var nextIndex = (selectedIndex + 1) % itemCount;

        // Set the selected tab
        ImagesTabControl.SelectedIndex = nextIndex;
    }

    private void ImageInTab_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (sender is not Panel panel) return;
        if (panel.DataContext is not ImageTabViewModel tabViewModel) return;
        tabViewModel.ZoomImage(e.Delta.Y);
    }
}