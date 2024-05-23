using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using WaifuGallery.ViewModels;

namespace WaifuGallery.Controls;

public partial class FileExplorer : UserControl
{
    private FileExplorerViewModel? FileExplorerViewModel => DataContext as FileExplorerViewModel;

    public FileExplorer()
    {
        InitializeComponent();
        // FileExplorerControl.KeyDown += FileExplorer_OnKeyDown;
        // FileExplorerExpander.KeyDown += FileExplorer_OnKeyDown;
        // FileExplorerScrollViewer.KeyDown += FileExplorer_OnKeyDown;
        FileExplorerListBox.KeyDown += FileExplorer_OnKeyDown;
        ImagePreviewControl.PointerWheelChanged += InputElement_OnPointerWheelChanged;
        FileExplorerContent.PointerMoved += OnPointerMoved_ChangePreviewPosition;

        ScrollViewer.SetHorizontalScrollBarVisibility(FileExplorerListBox, ScrollBarVisibility.Disabled);
    }

    private void FileExplorer_OnKeyDown(object? sender, KeyEventArgs e)
    {
        // switch (e)
        // {
        //     case {Key: Key.Down}:
        //     case {Key: Key.Up}:
        //     case {Key: Key.Right}:
        //     case {Key: Key.Left}:
        //         e.Handled = true;
        //         return;
        // }
        //
        // FileExplorerViewModel?.HandleKeyboardEvent(e);
        // e.Handled = true;
    }

    private void InputElement_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        FileExplorerViewModel?.PreviewImageViewModel.ZoomPreview(e.Delta.Y);
    }

    public override void Render(DrawingContext context)
    {
        if (FileExplorerViewModel is null) return;
        var fileExplorerWidth = (int) FileExplorerListBox.Bounds.Size.Width;
        const int fileWidth = 174;
        FileExplorerViewModel.ColumnsCount = fileExplorerWidth / fileWidth;
    }

    private void OnPointerMoved_ChangePreviewPosition(object? sender, PointerEventArgs e)
    {
        // if (e.KeyModifiers is not KeyModifiers.Control) return;
        if (sender is not Grid grid) return;
        var point = e.GetPosition(grid);
        var size = grid.Bounds.Size;
        var previewImageWidth = (double) FileExplorerViewModel?.PreviewImageViewModel.PreviewImageWidth;
        var previewImageHeight = (double) FileExplorerViewModel?.PreviewImageViewModel.PreviewImageHeight;
        const int offset = 20;
        var xClamp = size.Width - previewImageWidth;
        var yClamp = size.Height - previewImageHeight;
        if (xClamp < 0) xClamp = 0;
        if (yClamp < 0) yClamp = 0;
        var x = Math.Clamp(point.X, 0, xClamp);
        var y = Math.Clamp(point.Y, 0, yClamp);
        var newPoint = new Point(x - offset, y - offset);
        FileExplorerViewModel?.SendMessageToStatusBar(
            $"PointerMoved: X:{point.X}, Y:{point.Y} - Width:{size.Width}, Height:{size.Height}");
        FileExplorerViewModel?.PreviewImageViewModel.ChangePreviewPosition(newPoint);
    }
}