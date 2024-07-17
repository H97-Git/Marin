using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using WaifuGallery.ViewModels.FileManager;

namespace WaifuGallery.Controls;

public partial class ImagePreview : UserControl
{
    #region Private Fields

    private PreviewImageViewModel? PreviewImageViewModel => DataContext as PreviewImageViewModel;
    public ImagePreview() => InitializeComponent();

    #endregion

    #region Private Methods

    private void ImagePreview_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var properties = e.GetCurrentPoint(this).Properties;

        if (properties.IsXButton1Pressed)
        {
            PreviewImageViewModel?.NextPreview();
        }

        if (properties.IsXButton2Pressed)
        {
            PreviewImageViewModel?.PreviousPreview();
        }

        if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed)
            PreviewImageViewModel?.HidePreview();
    }

    #endregion

    private void PreviewCanvas_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (PreviewImageViewModel is null) return;
        PreviewImageViewModel.IsZooming = true;
        e.Handled = true;
        PreviewImageViewModel.ZoomPreview(e.Delta.Y);
        Task.Delay(1000).ContinueWith(_ =>
        {
            Dispatcher.UIThread.InvokeAsync(() => PreviewImageViewModel.IsZooming = false);
        });
    }
}