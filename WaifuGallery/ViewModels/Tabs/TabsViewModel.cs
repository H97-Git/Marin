using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using WaifuGallery.Models;

namespace WaifuGallery.ViewModels.Tabs;

public class TabsViewModel : ViewModelBase
{
    #region Private Members

    private ObservableCollection<TabViewModelBase> _openTabs = [];
    private TabViewModelBase? _selectedTab;
    private ImageTabViewModel? _currentImageTabViewModel;
    private int _selectedTabIndex;
    private bool _isSelectedTabSettingsTab = true;
    private bool IsSettingsTabOpen => OpenTabs.Any(x => x is TabSettingsViewModel);

    #endregion

    #region Public Properties

    public ObservableCollection<TabViewModelBase> OpenTabs
    {
        get => _openTabs;
        set => this.RaiseAndSetIfChanged(ref _openTabs, value);
    }

    public TabViewModelBase? SelectedTab
    {
        get => _selectedTab;
        set
        {
            // if (_selectedTab?.Id == value?.Id) return;
            // IsSelectedTabSettingsTab = value is TabSettingsViewModel;
            // if (!IsSelectedTabSettingsTab)
            // {
            //     ImageTabViewModel = value as ImageTabViewModel;
            // }

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

    public Size ControlSize { get; set; }

    public event EventHandler<Command>? OnSendCommandToMainView;

    public ICommand CloseTabCommand =>
        ReactiveCommand.Create(CloseTab);

    #endregion

    #region CTOR

    public TabsViewModel()
    {
        OpenSettingsTab();
        SelectedTab = OpenTabs.First();
    }

    #endregion

    #region Private Methods

    private void SendCommandToMainView(Command command)
    {
        OnSendCommandToMainView?.Invoke(this, command);
    }

    private void CloseTab()
    {
        if (SelectedTab is null) return;
        if (SelectedTab is TabSettingsViewModel) return;
        OpenTabs.Remove(SelectedTab);
        if (OpenTabs.Count is 0)
        {
            ImageTabViewModel = null;
            return;
        }

        SelectedTab = OpenTabs.First();
        // if (SelectedTab is ImageTabViewModel imageTabViewModel)
        // ImageTabViewModel = imageTabViewModel;
    }

    private void AddTab(TabViewModelBase tab)
    {
        OpenTabs.Add(tab);
        // OpenTabs = new ObservableCollection<TabViewModelBase>(OpenTabs.OrderBy(x => x, new TabsComparer()));
        SelectedTab = OpenTabs.Single(x => x.Id == tab.Id);
        if (SelectedTab is ImageTabViewModel)
        {
            IsSelectedTabSettingsTab = false;
            ImageTabViewModel = SelectedTab as ImageTabViewModel;
            FitToHeight();
        }
        else
        {
            IsSelectedTabSettingsTab = true;
        }
    }


    private static ImageTabViewModel? CreateImageTabFromCommand(Guid id, Command command)
    {
        var imagesInPath = command.ImagesInPath;
        if (imagesInPath is null) return null;

        var index = 0;
        if (command.Type is CommandType.OpenImageInNewTab)
        {
            index = command.Index;
        }

        return new ImageTabViewModel(id, imagesInPath, index);
    }

    private void ResizeAllTabByHeight()
    {
        foreach (var tabViewModel in OpenTabs)
        {
            (tabViewModel as ImageTabViewModel)?.ResizeImageByHeight(ControlSize.Height);
        }
    }

    private void ResizeTabByHeight(ImageTabViewModel imageTabViewModel)
    {
        imageTabViewModel.ResizeImageByHeight(ControlSize.Height);
    }

    private void ResizeAllTabByWidth()
    {
        foreach (var tabViewModel in OpenTabs)
        {
            (tabViewModel as ImageTabViewModel)?.ResizeImageByWidth(ControlSize.Width);
        }
    }

    private void ResizeTabByWidth(ImageTabViewModel imageTabViewModel)
    {
        imageTabViewModel.ResizeImageByWidth(ControlSize.Width);
    }

    #endregion

    #region Public Methods

    public void SelectionChanged(SelectionChangedEventArgs e)
    {
        if (SelectedTab is ImageTabViewModel imageTabViewModel)
        {
            ImageTabViewModel = imageTabViewModel;
            IsSelectedTabSettingsTab = false;
            FitToHeight();
        }
        else
        {
            IsSelectedTabSettingsTab = true;
        }
    }

    public void FitToWidth()
    {
        if (ImageTabViewModel is null) return;
        ResizeTabByWidth(ImageTabViewModel);
    }

    public void FitToHeight()
    {
        if (ImageTabViewModel is null) return;
        ResizeTabByHeight(ImageTabViewModel);
    }

    public void AddImageTab(Command command)
    {
        var id = Guid.NewGuid();
        var imageTabViewModel = CreateImageTabFromCommand(id, command);
        if (imageTabViewModel is null) return;
        AddTab(imageTabViewModel);
        FitToHeight();
    }


    public void OpenSettingsTab()
    {
        if (IsSettingsTabOpen) return;
        AddTab(new TabSettingsViewModel(Guid.Empty));
    }

    public void SwitchTab()
    {
        // Calculate the index of the next tab
        var nextIndex = (SelectedTabIndex + 1) % OpenTabs.Count;

        // Set the selected tab
        SelectedTabIndex = nextIndex;
    }

    #endregion
}