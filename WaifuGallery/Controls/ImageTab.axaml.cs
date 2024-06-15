using System;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using WaifuGallery.ViewModels.Tabs;

namespace WaifuGallery.Controls;

public partial class ImageTab : UserControl
{
    private ImageTabViewModel? ImageTabViewModel => DataContext as ImageTabViewModel;

    public ImageTab()
    {
        InitializeComponent();
        SetZoomBorder();
    }

    private void SetZoomBorder()
    {
        ZoomBorder.ClipToBounds = true;
        ZoomBorder.KeyDown += ZoomBorder_OnKeyDown;
        // ZoomBorder.MaxOffsetX = 300;
        // ZoomBorder.MaxOffsetY = 300;
        ZoomBorder.MaxZoomX = 50;
        ZoomBorder.MaxZoomY = 50;
        // ZoomBorder.MinOffsetX = -100;
        // ZoomBorder.MinOffsetY = -100;
        ZoomBorder.MinZoomX = 0.4;
        ZoomBorder.MinZoomY = 0.4;
        ZoomBorder.PanButton = ButtonName.Right;
        ZoomBorder.Stretch = StretchMode.None;
        ZoomBorder.ZoomChanged += ZoomBorder_OnZoomChanged;
        ZoomBorder.ZoomSpeed = 1.5;
    }

    private void ZoomBorder_OnZoomChanged(object sender, ZoomChangedEventArgs e)
    {
        Console.WriteLine($"[ZoomChanged] {e.ZoomX} {e.ZoomY} {e.OffsetX} {e.OffsetY}");
    }

    private void ZoomBorder_OnKeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.F:
                ZoomBorder.Fill();
                break;
            case Key.U:
                ZoomBorder.Uniform();
                break;
            case Key.R:
                ZoomBorder.ResetMatrix();
                break;
            case Key.T:
                ZoomBorder.ToggleStretchMode();
                ZoomBorder.AutoFit();
                break;
        }
    }
}