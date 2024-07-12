using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using WaifuGallery.ViewModels.FileManager;

namespace WaifuGallery.Controls;

public partial class FileManager : UserControl
{
    #region Private Fields

    private FileManagerViewModel? FileManagerViewModel => DataContext as FileManagerViewModel;

    #endregion

    #region Private Methods

    private void InputElement_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        FileManagerViewModel?.PreviewImageViewModel.ZoomPreview(e.Delta.Y);
    }

    private void OnPointerMoved_ChangePreviewPosition(object? sender, PointerEventArgs e)
    {
        // if (e.KeyModifiers is not KeyModifiers.Control) return;
        if (sender is not Control control) return;
        if (FileManagerViewModel?.PreviewImageViewModel is null) return;
        var point = e.GetPosition(control);
        var newPoint = CalcNewPoint(control.Bounds.Size, point,
            FileManagerViewModel.PreviewImageViewModel.PreviewSize);

        // FileManagerViewModel?.SendMessageToStatusBar(
        //     $"PointerMoved: X:{point.X}, Y:{point.Y} - Width:{size.Width}, Height:{size.Height}");
        FileManagerViewModel?.PreviewImageViewModel.ChangePreviewPosition(newPoint);
    }

    private static Point CalcNewPoint(Size gridSize, Point pointerPosition, Size previewImageSize)
    {
        const int offset = 30;
        var x = gridSize.Width - previewImageSize.Width;
        var y = gridSize.Height - previewImageSize.Height;
        x -= offset;
        y -= offset;
        if (x < 0) x = 0;
        if (y < 0) y = 0;
        var xClamp = Math.Clamp(pointerPosition.X - offset, 0, x);
        var yClamp = Math.Clamp(pointerPosition.Y - offset, 0, y);
        return new Point(xClamp, yClamp);
    }

    #endregion

    #region Ctor

    public FileManager()
    {
        InitializeComponent();
        ImagePreviewControl.PointerWheelChanged += InputElement_OnPointerWheelChanged;
        FileManagerContent.PointerMoved += OnPointerMoved_ChangePreviewPosition;
    }

    #endregion

    #region Public Methods

    public override void Render(DrawingContext context)
    {
        if (FileManagerViewModel is null) return;
        var fileManagerWidth = (int) FileManagerListBox.Bounds.Size.Width;
        //Todo: This should be taken from settings (or something like that) since this value doesn't reflect the actual desired size of the control.
        const int fileWidth = 165;
        FileManagerViewModel.ColumnsCount = fileManagerWidth / fileWidth;
    }

    #endregion
}