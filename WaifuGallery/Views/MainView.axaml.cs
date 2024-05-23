using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media.Imaging;
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
        if (MainViewModel is {FileExplorerViewModel.IsFileExplorerExpanded: false}) return;
        if (sender is not Grid mainGrid) return;
        if (mainGrid.Children[1] is not Grid control) return;
        var expander = control.Children[1].GetControl<Expander>("FileExplorerExpander");
        if (!expander.IsPointerOver)
        {
            MainViewModel?.FileExplorerViewModel.ToggleFileExplorer();
        }
    }

    #endregion
}