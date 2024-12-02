using System;
using System.Windows.Input;
using Avalonia.Media;
using Marin.UI.ViewModels.Tabs.Preferences;
using ReactiveUI;
using Serilog;

namespace Marin.UI.ViewModels.Tabs;

public class PreferencesTabViewModel : TabViewModelBase
{
    private bool _isMainVisible = true;
    private string _selectedCategory;
    private IBrush _foreground = new SolidColorBrush {Color = Colors.White};

    private void HideAll()
    {
        IsMainVisible = false;
        FileManagerPreferencesViewModel.IsVisible = false;
        FullScreenPreferencesViewModel.IsVisible = false;
        GeneralPreferencesViewModel.IsVisible = false;
        HotKeysPreferencesViewModel.IsVisible = false;
        ImagePreviewPreferencesViewModel.IsVisible = false;
        StatusBarPreferencesViewModel.IsVisible = false;
        TabsPreferencesViewModel.IsVisible = false;
    }

    #region CTOR

    public PreferencesTabViewModel()
    {
        Id = Guid.Empty.ToString();
        Header = "Preferences";
    }

    #endregion

    #region Public Properties

    public FileManagerPreferencesViewModel FileManagerPreferencesViewModel { get; } = new();

    public FullScreenPreferencesViewModel FullScreenPreferencesViewModel { get; } = new();

    public GeneralPreferencesViewModel GeneralPreferencesViewModel { get; } = new();

    public HotKeysPreferencesViewModel HotKeysPreferencesViewModel { get; } = new();

    public ImagePreviewPreferencesViewModel ImagePreviewPreferencesViewModel { get; } = new();

    public StatusBarPreferencesViewModel StatusBarPreferencesViewModel { get; } = new();

    public TabsPreferencesViewModel TabsPreferencesViewModel { get; } = new();

    
    public string SelectedCategory
    {
        get => _selectedCategory;
        set => this.RaiseAndSetIfChanged(ref _selectedCategory, value);
    }

    public bool IsMainVisible
    {
        get => _isMainVisible;
        set => this.RaiseAndSetIfChanged(ref _isMainVisible, value);
    }
    
    public IBrush Foreground
    {
        get => _foreground;
        set => this.RaiseAndSetIfChanged(ref _foreground, value);
    }

    public void GoToMainMenu()
    {
        HideAll();
        SelectedCategory = "";
        Foreground = new SolidColorBrush() {Color = Colors.White};
        IsMainVisible = true;
    }

    public ICommand SwitchCategory => ReactiveCommand.Create<string>(x =>
    {
        Log.Debug("SwitchCategory: {X}", x);
        Foreground = new SolidColorBrush() {Color = Colors.Thistle};
        switch (x)
        {
            case "General":
                HideAll();
                GeneralPreferencesViewModel.IsVisible = true;
                SelectedCategory = " > General";
                break;

            case "HotKeys":
                HideAll();
                HotKeysPreferencesViewModel.IsVisible = true;
                SelectedCategory = " > HotKeys";
                break;

            case "ImagePreview":
                HideAll();
                ImagePreviewPreferencesViewModel.IsVisible = true;
                SelectedCategory = " > Image Preview";
                break;

            case "StatusBar":
                HideAll();
                StatusBarPreferencesViewModel.IsVisible = true;
                SelectedCategory = " > Status Bar";
                break;

            case "Tabs":
                HideAll();
                TabsPreferencesViewModel.IsVisible = true;
                SelectedCategory = " > Tabs";
                break;

            case "FullScreen":
                HideAll();
                FullScreenPreferencesViewModel.IsVisible = true;
                SelectedCategory = " > FullScreen";
                break;

            case "FileManager":
                HideAll();
                FileManagerPreferencesViewModel.IsVisible = true;
                SelectedCategory = " > File Manager";
                break;
        }
    });

    #endregion
}