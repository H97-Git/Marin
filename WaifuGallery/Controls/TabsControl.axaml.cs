using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using WaifuGallery.ViewModels;
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

    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        // Find which tab is clicked
        if (sender is not ListBox listBox) return;
        if (listBox.SelectedIndex < 0) return;
        if (listBox.Items[listBox.SelectedIndex] is not ImageTabViewModel imageTabViewModel) return;
        // Set the content of the tab
        if (TabsViewModel is null) return;

        TabsViewModel.ImageTabViewModel = imageTabViewModel;
        // var image = new Bitmap(tab.CurrentImagePath);
        // ImageTabContent.Source = image;
    }


    private void ImagesTabControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) =>
        TabsViewModel?.FitToHeight(TabsUserControl.Bounds.Size.Height);


    private void ImageInTab_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (sender is not Grid grid) return;
        if (grid.DataContext is not TabsViewModel tabViewModel) return;
        tabViewModel.ImageTabViewModel?.ZoomImage(e.Delta.Y);
    }
}