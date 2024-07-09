using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using WaifuGallery.ViewModels.Dialogs;

namespace WaifuGallery.Controls.Dialogs;

public partial class KeyboardKeySetter : UserControl
{
    #region Private Methods

    private void InputElement_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key is Key.Escape)
        {
            OnEscapePressed?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (DataContext is not KeyboardKeySetterViewModel viewModel) return;
        viewModel.KeyEventArgs = e;
        e.Handled = true;
    }

    private void Input_OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        // We will set the focus into our input field just after it got attached to the visual tree.
        if (sender is TextBox textBox)
        {
            Dispatcher.UIThread.InvokeAsync(() => { textBox.Focus(); });
        }
    }

    #endregion

    #region CTOR

    public KeyboardKeySetter()
    {
        InitializeComponent();
    }

    #endregion
    public EventHandler? OnEscapePressed { get; init; }
}