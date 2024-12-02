using Avalonia.Controls;
using Marin.UI.ViewModels.Tabs.Preferences;

namespace Marin.UI.Controls.Tabs.Preferences;

public partial class FileManager : UserControl
{
    private FileManagerPreferencesViewModel? FileManagerPreferencesViewModel =>
        DataContext as FileManagerPreferencesViewModel;

    public FileManager()
    {
        InitializeComponent();
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox comboBox) return;
        var value = (comboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
        if (FileManagerPreferencesViewModel != null)
            FileManagerPreferencesViewModel.DefaultSortOrder = value;
    }
}