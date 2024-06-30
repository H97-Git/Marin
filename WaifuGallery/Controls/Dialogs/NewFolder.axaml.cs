using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using WaifuGallery.ViewModels.Dialogs;

namespace WaifuGallery.Controls.Dialogs;

public partial class NewFolder : UserControl
{
    public NewFolder()
    {
        InitializeComponent();
    }

    public EventHandler? OnEnterPressed { get; init; }

    private void Input_OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        // We will set the focus into our input field just after it got attached to the visual tree.
        if (sender is TextBox textBox)
        {
            Dispatcher.UIThread.InvokeAsync(() => { textBox.Focus(); });
        }
    }

    private void InputElement_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not NewFolderViewModel newFolderViewModel) return;
        if (string.IsNullOrWhiteSpace(newFolderViewModel.NewFolderName)) return;
        if (e.Key is Key.Enter)
        {
            OnEnterPressed?.Invoke(this, EventArgs.Empty);
        }
    }
}