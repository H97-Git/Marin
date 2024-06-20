using System;
using Avalonia.Input;
using FluentAvalonia.UI.Windowing;
using WaifuGallery.ViewModels;
using WaifuGallery.Views;

namespace WaifuGallery;

public partial class MainWindow : AppWindow
{
    private MainViewViewModel? MainViewViewModel => (Content as MainView)?.DataContext as MainViewViewModel;

    public MainWindow()
    {
        Content = new MainView()
        {
            DataContext = new MainViewViewModel()
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