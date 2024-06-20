using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
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

    private void Input_OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        // We will set the focus into our input field just after it got attached to the visual tree.
        if (sender is TextBox textBox)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                textBox.Focus(NavigationMethod.Unspecified, KeyModifiers.None);
            });
        }
    }
}