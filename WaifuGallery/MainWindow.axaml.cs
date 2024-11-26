using Avalonia.Controls;
using Avalonia.Input;
using WaifuGallery.ViewModels;
using WaifuGallery.Views;

namespace WaifuGallery;

public partial class MainWindow : Window
{
    private MainViewViewModel? MainViewViewModel => (Content as MainView)?.DataContext as MainViewViewModel;

    public MainWindow()
    {
        KeyDown += OnKeyDown;
        InitializeComponent();
    }

    /// <summary>
    /// Send all keyboard events from the MainWindow to the MainViewModel.
    /// </summary>
    /// <param name="sender">MainWindow</param>
    /// <param name="e">KeyEventArgs</param>
    private void OnKeyDown(object? sender, KeyEventArgs e) => MainViewViewModel?.HandleKeyBoardEvent(e);
}