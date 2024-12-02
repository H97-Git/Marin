using System;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using FluentAvalonia.UI.Controls;
using Marin.UI.Commands;
using Marin.UI.ViewModels.Tabs;
using ReactiveUI;
using Serilog;

namespace Marin.UI.Controls.Tabs;

public partial class ImageTab : UserControl
{
    #region Private Fields

    private ImageTabViewModel? ImageTabViewModel => DataContext as ImageTabViewModel;
    private static double MinZoom => 0.4;

    #endregion

    #region Private Methods

    private void SetZoomProperties()
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
        // Console.WriteLine($"[ZoomChanged] {e.ZoomX} {e.ZoomY} {e.OffsetX} {e.OffsetY}");
        if (ImageTabViewModel is null) return;
        ImageTabViewModel.Matrix = ZoomBorder.Matrix;
        ImageTabViewModel.IsDefaultZoom = false;
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
            case Key.R:
                ZoomBorder.ResetMatrix();
                break;
            // case Key.F:
            //     ZoomBorder.Fill();
            //     break;
            // case Key.U:
            //     ZoomBorder.Uniform();
            //     break;
            // case Key.T:
            //     ZoomBorder.ToggleStretchMode();
            //     break;
            // case Key.A:
            //     ZoomBorder.AutoFit();
            //     break;
        }
    }

    #endregion

    #region CTOR

    public ImageTab()
    {
        InitializeComponent();
        SetZoomProperties();
        MessageBus.Current.Listen<ResetZoomCommand>()
            .Subscribe(_ => ZoomBorder.ResetMatrix());
        MessageBus.Current.Listen<SetZoomCommand>()
            .Subscribe(x => ZoomBorder.SetMatrix(x.Matrix));
    }

    #endregion

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
       
        if (point.Properties.IsXButton1Pressed)
        {
            // MessageBus.Current.SendMessage(new SendMessageToStatusBarCommand(InfoBarSeverity.Informational,"XButton1Pressed"));
            ImageTabViewModel?.LoadNextImage();
        }

        if (point.Properties.IsXButton2Pressed)
        {
            // MessageBus.Current.SendMessage(new SendMessageToStatusBarCommand(InfoBarSeverity.Informational,"XButton2Pressed"));
            ImageTabViewModel?.LoadPreviousImage();
        }
    }

    private void InputElement_OnKeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.G:
                ImageTabViewModel?.ToggleGrid();
                break;
            case Key.Escape:
                ImageTabViewModel?.CloseGrid();
                break;
        }
    }

    private void PreviewList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        Log.Debug("[SelectionChanged] {Index}", (e.Source as ListBox)?.SelectedIndex);
        ImageTabViewModel?.GridSelected((e.Source as ListBox)?.SelectedIndex);
    }
}