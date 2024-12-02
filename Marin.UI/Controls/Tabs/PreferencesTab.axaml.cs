using Avalonia.Controls;
using Avalonia.Input;
using Marin.UI.ViewModels.Tabs;

namespace Marin.UI.Controls.Tabs;

public partial class PreferencesTab : UserControl
{
    public PreferencesTab()
    {
        InitializeComponent();
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            (DataContext as PreferencesTabViewModel)?.GoToMainMenu();
        }
    }
}