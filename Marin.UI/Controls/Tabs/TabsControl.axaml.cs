﻿using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Marin.UI.ViewModels.Tabs;
using Serilog;

namespace Marin.UI.Controls.Tabs;

public partial class TabsControl : UserControl
{
    #region Private Fields

    private TabsViewModel? TabsViewModel => DataContext as TabsViewModel;
    private bool _pressedTab;
    private Point _pressedTabPosition;
    private bool _startDrag;

    #endregion

    #region Private Methods

    private void ImagesTabControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) =>
        TabsViewModel?.SelectionChanged(e);

    private void ScrollTabs(object? sender, PointerWheelEventArgs e)
    {
        Log.Debug("ScrollTabs");
        if (!e.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            if (e.Delta.Y < 0)
                LauncherTabsScroller.LineRight();
            else if (e.Delta.Y > 0)
                LauncherTabsScroller.LineLeft();
            e.Handled = true;
        }
    }

    private void OnPointerPressedCloseTab(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control control) return;
        if (!e.GetCurrentPoint(control).Properties.IsMiddleButtonPressed) return;
        Log.Debug("CloseTab on middle click");
        var id = "";
        foreach (var listBoxItem in control.GetVisualChildren().OfType<ListBoxItem>())
        {
            if (listBoxItem is {IsPointerOver: true, DataContext: ImageTabViewModel imageTabViewModel})
            {
                id = imageTabViewModel.Id;
            }

            if (listBoxItem is {IsPointerOver: true, DataContext: PreferencesTabViewModel preferencesTabViewModel})
            {
                id = preferencesTabViewModel.Id;
            }
        }

        if (id == "") return;

        TabsViewModel?.CloseTab(id);
    }

    private void OnPointerPressedStartMoveTab(object? sender, PointerPressedEventArgs e)
    {
        _pressedTab = true;
        _startDrag = false;
        _pressedTabPosition = e.GetPosition(sender as Border);
    }

    private void OnPointerMovedOverTab(object? sender, PointerEventArgs e)
    {
        if (_pressedTab && !_startDrag && sender is Border border)
        {
            var delta = e.GetPosition(border) - _pressedTabPosition;
            var sizeSquired = delta.X * delta.X + delta.Y * delta.Y;
            if (sizeSquired < 64)
                return;

            _startDrag = true;

            var data = new DataObject();
            data.Set("MovedTab", border.DataContext);
            DragDrop.DoDragDrop(e, data, DragDropEffects.Move);
        }

        e.Handled = true;
    }

    private void OnPointerReleasedTab(object? sender, PointerReleasedEventArgs e)
    {
        _pressedTab = false;
        _startDrag = false;
    }

    private void OnLoaded_SetupDragAndDrop(object? sender, RoutedEventArgs e)
    {
        var id = ((sender as Control)?.DataContext as TabViewModelBase)?.Id;
        Log.Debug("SetupDragAndDrop : {I}", id);
        if (sender is Border border)
        {
            DragDrop.SetAllowDrop(border, true);
            border.AddHandler(DragDrop.DropEvent, DropTab);
        }

        e.Handled = true;
    }

    private void DropTab(object? sender, DragEventArgs e)
    {
        Log.Debug("DropTab");
        if (e.Data.Contains("MovedTab") && sender is Border border)
        {
            var to = border.DataContext as TabViewModelBase;
            var moved = e.Data.Get("MovedTab") as TabViewModelBase;
            if (to != null && moved != null && to != moved && TabsViewModel is not null)
            {
                TabsViewModel.MoveTab(moved, to);
            }
        }

        _pressedTab = false;
        _startDrag = false;
        e.Handled = true;
    }

    #endregion

    #region CTOR

    public TabsControl()
    {
        InitializeComponent();
        ImagesTabControl.SelectionChanged += ImagesTabControl_OnSelectionChanged;
        ImagesTabControl.AddHandler(KeyDownEvent, ImagesTabControl_OnKeyDown, RoutingStrategies.Tunnel);
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        if (TabsViewModel is null) return;
        TabsViewModel.ControlSize = TabsUserControl.Bounds.Size;
    }

    #endregion

    private void ImagesTabControl_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key is Key.Left or Key.Right)
        {
            e.Handled = true;
        }
    }
}