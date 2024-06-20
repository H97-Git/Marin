using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using WaifuGallery.ViewModels;

namespace WaifuGallery.Controls;

public partial class KeyboardKeySetter : UserControl
{
    public KeyboardKeySetter()
    {
        InitializeComponent();
    }

    private void InputElement_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key is Key.Escape) return;
        if (DataContext is not KeyboardKeySetterViewModel viewModel) return;
        viewModel.Key = e.Key;
        
        e.Handled = true;
    }
}