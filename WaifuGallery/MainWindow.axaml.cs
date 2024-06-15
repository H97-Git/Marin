using System;
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
        Content = new MainView()
        {
            DataContext = new MainViewViewModel(this)
        };
        KeyDown += OnKeyDown;
        InitializeComponent();
    }

    /// <summary>
    /// Send all keyboard events from the MainWindow to the MainViewModel.
    /// </summary>
    /// <param name="sender">MainWindow</param>
    /// <param name="e">KeyEventArgs</param>
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        Console.WriteLine(e.KeyModifiers is KeyModifiers.None
            ? $"{e.Key} MainWindow"
            : $"{e.Key} {e.KeyModifiers} MainWindow");
        MainViewViewModel?.HandleKeyBoardEvent(e);
    }
}