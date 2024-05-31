using System;
using Avalonia.Controls;
using Avalonia.Input;
using WaifuGallery.ViewModels;

namespace WaifuGallery.Views;

public partial class MainView : UserControl
{
    #region Private Members

    private MainViewViewModel? MainViewModel => DataContext as MainViewViewModel;

    #endregion

    #region CTOR

    public MainView()
    {
        InitializeComponent();
        MainGrid.PointerPressed += MainGrid_OnPointerPressed;
    }

    #endregion

    #region Private Methods

    private void MainGrid_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        switch (MainViewModel)
        {
            // If the file explorer is not expanded or the menu is open, don't do anything.
            case {FileExplorerViewModel.IsFileExplorerExpanded: false}:
            case {MenuBarViewModel.IsMenuOpen: true}:
                return;
        }

        // if (sender is not Grid mainGrid) return;
        // if (mainGrid.Children[1] is not Grid control) return;
        // var expander = control.Children[1].GetControl<Expander>("FileExplorerExpander");
        // if (!expander.IsPointerOver)
        // {
        //     MainViewModel?.FileExplorerViewModel.ToggleFileExplorer();
        // }
    }

    #endregion

    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Middle) return;
        switch (MainViewModel)
        {
            case {FileExplorerViewModel.IsFileExplorerExpanded: false}:
            case {FileExplorerViewModel.IsFileExplorerVisible: false}:
            case {FileExplorerViewModel.IsPointerOver: true}:
                return;
            default:
                MainViewModel?.FileExplorerViewModel.ToggleFileExplorer();
                break;
        }
    }
}