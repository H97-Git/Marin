using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using ReactiveUI;
using WaifuGallery.Commands;
using WaifuGallery.ViewModels.Tabs;

namespace WaifuGallery.Controls;

public partial class ImageTab : UserControl
{
    private ImageTabViewModel? ImageTabViewModel => DataContext as ImageTabViewModel;
    private double MinZoom => 0.4;

    public ImageTab()
    {
        InitializeComponent();
        SetZoomBorder();
        MessageBus.Current.Listen<ResetZoomCommand>()
            .Subscribe(_ => ZoomBorder.ResetMatrix());
    }

    private void SetZoomBorder() 
    {
        ZoomBorder.EnablePan = true;
        ZoomBorder.KeyDown += ZoomBorder_OnKeyDown;
        ZoomBorder.MaxZoomX = 50;
        ZoomBorder.MaxZoomY = 50;
        // ZoomBorder.MaxOffsetX = 300;
        // ZoomBorder.MaxOffsetY = 300;
        // ZoomBorder.MinOffsetX = -100;
        // ZoomBorder.MinOffsetY = -100;
        ZoomBorder.MinZoomX = MinZoom;
        ZoomBorder.MinZoomY = MinZoom;
        ZoomBorder.PanButton = ButtonName.Right;
        ZoomBorder.Stretch = StretchMode.None;
        ZoomBorder.ZoomChanged += ZoomBorder_OnZoomChanged;
        ZoomBorder.ZoomSpeed = 1.5;
    }

    private void ZoomBorder_OnZoomChanged(object sender, ZoomChangedEventArgs e)
    {
        Console.WriteLine($"[ZoomChanged] {e.ZoomX} {e.ZoomY} {e.OffsetX} {e.OffsetY}");
        // if (e.ZoomX > 1.0 || e.ZoomY > 1.0)
        // {
        //     ZoomBorder.EnablePan = true;
        // }
        // else
        // {
        //     ZoomBorder.EnablePan = false;
        // }
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