using System;
using System.Windows.Input;
using Avalonia;
using ReactiveUI;
using Serilog;
using WaifuGallery.ViewModels.Tabs.Preferences;

namespace WaifuGallery.ViewModels.Tabs;

public class PreferencesTabViewModel : TabViewModelBase
{
    private bool _isMainVisible = true;
    private string _selectedCategory;

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

    private void ShowAll()
    {
        IsMainVisible = true;
        FileManagerPreferencesViewModel.IsVisible = true;
        FullScreenPreferencesViewModel.IsVisible = true;
        GeneralPreferencesViewModel.IsVisible = true;
        HotKeysPreferencesViewModel.IsVisible = true;
        ImagePreviewPreferencesViewModel.IsVisible = true;
        StatusBarPreferencesViewModel.IsVisible = true;
        TabsPreferencesViewModel.IsVisible = true;
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

    public string CurrentVersion => "0.0.1";

    public string? CurrentAvaloniaVersion =>
        typeof(Application).Assembly.GetName().Version?.ToString();

    public void GoToMainMenu()
    {
        HideAll();
        IsMainVisible = true;
    }

    public ICommand SwitchCategory => ReactiveCommand.Create<string>(x =>
    {
        Log.Debug("SwitchCategory: {X}", x);
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