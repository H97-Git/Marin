using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using WaifuGallery.ViewModels.Dialogs;

namespace WaifuGallery.Controls.Dialogs;

public partial class UserInput : UserControl
{
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
        if (DataContext is not UserInputDialog newFolderViewModel) return;
        if (string.IsNullOrWhiteSpace(newFolderViewModel.UserInput)) return;
        switch (e.Key)
        {
            case Key.Enter:
                OnEnterPressed?.Invoke(this, EventArgs.Empty);
                break;
            case Key.Escape:
                OnEscapePressed?.Invoke(this, EventArgs.Empty);
                break;
        }
    }

    public UserInput()
    {
        InitializeComponent();
    }

    public EventHandler? OnEnterPressed { get; init; }
    public EventHandler? OnEscapePressed { get; init; }
}