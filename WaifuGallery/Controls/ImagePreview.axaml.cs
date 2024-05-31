using Avalonia.Controls;
using Avalonia.Input;
using WaifuGallery.ViewModels;
using WaifuGallery.ViewModels.FileExplorer;

namespace WaifuGallery.Controls;

public partial class ImagePreview : UserControl
{
    private PreviewImageViewModel? PreviewImageViewModel => DataContext as PreviewImageViewModel;
    public ImagePreview() => InitializeComponent();

    private void ImagePreview_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var properties = e.GetCurrentPoint(this).Properties;
        if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed)
            PreviewImageViewModel?.ClosePreview();
    }
}