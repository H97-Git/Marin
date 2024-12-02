using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Marin.UI.Models;
using Marin.UI.ViewModels.FileManager;

namespace Marin.UI.Controls.FileManager;

public partial class FileManager : UserControl
{
    #region Private Fields

    private FileManagerViewModel? FileManagerViewModel => DataContext as FileManagerViewModel;

    #endregion

    #region Private Methods

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
        FileManagerViewModel.PreviewImageViewModel.ChangePreviewPosition(newPoint);
    }

    private static Point CalcNewPoint(Size panelSize, Point pointerPosition, Size previewImageSize)
    {
        const int offset = 30;

        // Calculate the initial position to center the preview image around the mouse pointer
        var initialX = pointerPosition.X - previewImageSize.Width / 2;
        var initialY = pointerPosition.Y - previewImageSize.Height / 2;

        // Adjust to ensure the preview image stays within the bounds of the panel
        var maxX = panelSize.Width - previewImageSize.Width - offset;
        var maxY = panelSize.Height - previewImageSize.Height - offset;
        
        if(maxX < offset) maxX = offset;
        if(maxY < offset) maxY = offset;

        var x = Math.Clamp(initialX, offset, maxX);
        var y = Math.Clamp(initialY, offset, maxY);
        
        return new Point(x, y);
    }

    #endregion

    #region CTOR

    public FileManager()
    {
        InitializeComponent();
        FileManagerContent.PointerMoved += OnPointerMoved_ChangePreviewPosition;
    }

    #endregion

    #region Public Methods

    public override void Render(DrawingContext context)
    {
        if (FileManagerViewModel is null) return;
        var fileManagerWidth = (int) FileManagerListBox.Bounds.Size.Width;
        var fileWidth = Settings.Instance.FileManagerPreference.FileWidth;
        FileManagerViewModel.ColumnsCount = fileManagerWidth / (fileWidth + 28); // 28 is the padding
    }

    #endregion

    private void FileManagerListBox_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        // if(FileManagerViewModel is null) return;
        // if (FileManagerViewModel.PreviewImageViewModel.IsPreviewImageVisible)
        // {
        //     e.Handled = true;
        // }
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (FileManagerViewModel != null)
            FileManagerViewModel.SortBy = ((sender as ComboBox)?.SelectedItem as ComboBoxItem)?.Content?.ToString();
    }
}