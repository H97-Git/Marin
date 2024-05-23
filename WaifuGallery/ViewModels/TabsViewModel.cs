using System;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using WaifuGallery.Helpers;
using WaifuGallery.Models;

namespace WaifuGallery.ViewModels;

public class TabsViewModel : ViewModelBase
{
    #region Private Members

    private ObservableCollection<TabViewModel> _openTabs = [];
    private TabViewModel? _selectedTab;
    private readonly MainWindow _mainWindow;
    private bool _isSelectedTabSettingsTab = true;

    #endregion

    #region Public Properties

    public Func<object>? PreventLastTabCloseWindow { get; set; } = null;

    public ObservableCollection<TabViewModel> OpenTabs
    {
        get => _openTabs;
        set => this.RaiseAndSetIfChanged(ref _openTabs, value);
    }

    public TabViewModel? SelectedTab
    {
        get => _selectedTab;
        set
        {
            IsSelectedTabSettingsTab = value is TabSettingsViewModel;
            this.RaiseAndSetIfChanged(ref _selectedTab, value);
        }
    }

    public bool IsSelectedTabSettingsTab
    {
        get => _isSelectedTabSettingsTab;
        set => this.RaiseAndSetIfChanged(ref _isSelectedTabSettingsTab, value);
    }

    #endregion

    #region CTOR

    public TabsViewModel(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        OpenTabs.Add(new TabSettingsViewModel
        {
            Header = "Settings"
        });
    }

    #endregion

    public event EventHandler<Command> OnSendCommandToMainView;

    private void SendCommandToMainView(Command command)
    {
        OnSendCommandToMainView.Invoke(this, command);
    }

    public void CloseTab(string currentImagePath)
    {
        foreach (var tabViewModel in OpenTabs)
        {
            if (tabViewModel is not ImageTabViewModel imageTabViewModel) continue;
            if (imageTabViewModel.ImagesInPath.Any(x => x == currentImagePath))
            {
                OpenTabs.Remove(tabViewModel);
            }
        }
    }

    public void FitToWidth(double size) =>
        ResizeAllTabByWidth(size);

    public void FitToHeight(double size) =>
        ResizeAllTabByHeight(size);

    public void AddTab(TabViewModel tab)
    {
        OpenTabs.Add(tab);
        OpenTabs = new ObservableCollection<TabViewModel>(OpenTabs.OrderBy(x => x, new TabsComparer()));
        SelectedTab = OpenTabs.First();
    }

    private void ResizeAllTabByHeight(double newSizeHeight)
    {
        foreach (var tabViewModel in OpenTabs)
        {
            var imageTabViewModel = tabViewModel as ImageTabViewModel;
            imageTabViewModel?.ResizeImageByHeight(newSizeHeight);
            // tabViewModel.ResizeImageByHeight(newSizeHeight);
        }
    }

    private void ResizeAllTabByWidth(double newSizeWidth)
    {
        foreach (var tabViewModel in OpenTabs)
        {
            var imageTabViewModel = tabViewModel as ImageTabViewModel;
            imageTabViewModel?.ResizeImageByWidth(newSizeWidth);
            // tabViewModel.ResizeImageByWidth(newSizeWidth);
        }
    }
}