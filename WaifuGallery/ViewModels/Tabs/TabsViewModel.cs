using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;
using WaifuGallery.Helpers;
using WaifuGallery.Models;

namespace WaifuGallery.ViewModels.Tabs;

public class TabsViewModel : ViewModelBase
{
    #region Private Members

    private ObservableCollection<TabViewModelBase> _openTabs = [];
    private TabViewModelBase _selectedTab;
    private int _selectedTabIndex;
    private ImageTabViewModel? _currentImageTabViewModel;
    private bool _isSelectedTabSettingsTab = true;

    #endregion

    #region Public Properties

    public ObservableCollection<TabViewModelBase> OpenTabs
    {
        get => _openTabs;
        set => this.RaiseAndSetIfChanged(ref _openTabs, value);
    }

    public TabViewModelBase SelectedTab
    {
        get => _selectedTab;
        set
        {
            IsSelectedTabSettingsTab = value is TabSettingsViewModel;
            this.RaiseAndSetIfChanged(ref _selectedTab, value);
        }
    }

    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedTabIndex, value);
    }

    public bool IsSelectedTabSettingsTab
    {
        get => _isSelectedTabSettingsTab;
        set => this.RaiseAndSetIfChanged(ref _isSelectedTabSettingsTab, value);
    }

    public ImageTabViewModel? ImageTabViewModel
    {
        get => _currentImageTabViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentImageTabViewModel, value);
    }

    public TabSettingsViewModel? TabSettingsViewModel => SelectedTab as TabSettingsViewModel;
    public event EventHandler<Command> OnSendCommandToMainView;

    public ICommand CloseTabCommand =>
        ReactiveCommand.Create(CloseTab);

    #endregion

    #region CTOR

    public TabsViewModel()
    {
        OpenTabs.Add(new TabSettingsViewModel
        {
            Header = "Settings"
        });

        _selectedTab = OpenTabs.First();
    }

    #endregion

    private void SendCommandToMainView(Command command)
    {
        OnSendCommandToMainView.Invoke(this, command);
    }

    private void CloseTab()
    {
        if (SelectedTab is TabSettingsViewModel) return;
        OpenTabs.Remove(SelectedTab);
        SelectedTab = OpenTabs.First();
        if (SelectedTab is ImageTabViewModel imageTabViewModel)
            ImageTabViewModel = imageTabViewModel;
    }

    public void FitToWidth(double size) =>
        ResizeAllTabByWidth(size);

    public void FitToHeight(double size) =>
        ResizeAllTabByHeight(size);

    public void AddTab(TabViewModelBase tab)
    {
        OpenTabs.Add(tab);
        OpenTabs = new ObservableCollection<TabViewModelBase>(OpenTabs.OrderBy(x => x, new TabsComparer()));
        SelectedTab = OpenTabs.First();
        ImageTabViewModel = SelectedTab as ImageTabViewModel;
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

    public void SwitchTab()
    {
        // Calculate the index of the next tab
        var nextIndex = (SelectedTabIndex + 1) % OpenTabs.Count;

        // Set the selected tab
        SelectedTabIndex = nextIndex;
    }
}