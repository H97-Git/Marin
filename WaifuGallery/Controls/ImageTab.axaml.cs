using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using WaifuGallery.ViewModels.Tabs;

namespace WaifuGallery.Controls;

public partial class ImageTab : UserControl
{
    private bool _isMouseDown;
    private Point _startDragPosition;
    private Point _initialImagePosition;

    private ImageTabViewModel? ImageTabViewModel => DataContext as ImageTabViewModel;

    public ImageTab()
    {
        InitializeComponent();
        ImageCanvas.PointerPressed += Canvas_OnPointerPressed;
        ImageCanvas.PointerReleased += Canvas_OnPointerReleased;
        ImageCanvas.PointerMoved += Canvas_OnPointerMoved;
    }

    private void Canvas_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isMouseDown) return;

        var currentPosition = e.GetPosition(ImageCanvas);
        var delta = _startDragPosition - currentPosition;

        if (ImageTabViewModel is null) return;
        ImageTabViewModel.ImagePosition =
            new Point(_initialImagePosition.X - delta.X, _initialImagePosition.Y - delta.Y);
    }

    private void Canvas_OnPointerReleased(object? sender, PointerReleasedEventArgs e) => _isMouseDown = false;


    private void Canvas_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // Check if the mouse is over the image
        if (!e.GetCurrentPoint(ImageCanvas).Properties.IsLeftButtonPressed) return;

        _isMouseDown = true;
        _startDragPosition = e.GetPosition(ImageCanvas);
        _initialImagePosition = new Point(Canvas.GetLeft(ImageInTab), Canvas.GetTop(ImageInTab));
    }
}