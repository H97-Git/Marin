﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using WaifuGallery.Commands;

namespace WaifuGallery.ViewModels.Tabs;

public class TabsViewModel : ViewModelBase
{
    #region Private Members

    private ObservableCollection<TabViewModelBase> _openTabs = [];
    private TabViewModelBase? _selectedTab;
    private ImageTabViewModel? _currentImageTabViewModel;
    private TabSettingsViewModel? _tabSettingsViewModel;
    private int _selectedTabIndex;
    private bool _isSettingsTabVisible = true;
    private bool _isImageTabVisible;
    private bool _isTabHeadersVisible = true;
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
        set => this.RaiseAndSetIfChanged(ref _selectedTab, value);
    }

    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedTabIndex, value);
    }

    public bool IsSettingsTabVisible
    {
        get => _isSettingsTabVisible;
        set => this.RaiseAndSetIfChanged(ref _isSettingsTabVisible, value);
    }

    public bool IsImageTabVisible
    {
        get => _isImageTabVisible;
        set => this.RaiseAndSetIfChanged(ref _isImageTabVisible, value);
    }

    public bool IsTabHeadersVisible
    {
        get => _isTabHeadersVisible;
        set => this.RaiseAndSetIfChanged(ref _isTabHeadersVisible, value);
    }

    public ImageTabViewModel? ImageTabViewModel
    {
        get => _currentImageTabViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentImageTabViewModel, value);
    }

    public TabSettingsViewModel? TabSettingsViewModel
    {
        get => _tabSettingsViewModel;
        set => this.RaiseAndSetIfChanged(ref _tabSettingsViewModel, value);
    }

    public Size ControlSize { get; set; }

    public ICommand CloseTabCommand =>
        ReactiveCommand.Create(CloseTab);

    #endregion

    #region CTOR

    public TabsViewModel()
    {
        OpenSettingsTab();
        SelectedTab = OpenTabs.First();
        MessageBus.Current.Listen<OpenFileCommand>().Subscribe(async x => await OpenFile(x));
        MessageBus.Current.Listen<OpenInNewTabCommand>().Subscribe(AddImageTab);
        MessageBus.Current.Listen<FitToHeightCommand>().Subscribe(_ => FitToHeightAndResetZoom());
        MessageBus.Current.Listen<FitToWidthCommand>().Subscribe(_ => FitToWidthAndResetZoom());
        MessageBus.Current.Listen<OpenSettingsTabCommand>().Subscribe(_ => OpenSettingsTab());
    }

    #endregion

    #region Private Methods

    private void SendMessageToStatusBar(string message)
    {
        var command = new SendMessageToStatusBarCommand(InfoBarSeverity.Informational, message);
        SendCommandToMessageBus(command);
    }

    private void SendCommandToMessageBus(ICommandMessage command)
    {
        MessageBus.Current.SendMessage(command);
    }

    private void CloseTab()
    {
        if (SelectedTab is null) return;
        if (SelectedTab is TabSettingsViewModel && !Preferences.Instance.IsTabSettingsClosable) return;
        OpenTabs.Remove(SelectedTab);
        if (OpenTabs.Count is 0)
        {
            TabSettingsViewModel = null;
            ImageTabViewModel = null;
            IsSettingsTabVisible = false;
            IsImageTabVisible = false;
            return;
        }

        SelectedTab = OpenTabs.First();
    }

    private void AddTab(TabViewModelBase tab)
    {
        OpenTabs.Add(tab);
        SelectedTab = OpenTabs.First(x => x.Id == tab.Id);
        if (SelectedTab is ImageTabViewModel)
        {
            IsSettingsTabVisible = false;
            IsImageTabVisible = true;
            ImageTabViewModel = SelectedTab as ImageTabViewModel;
        }
        else
        {
            IsSettingsTabVisible = true;
            IsImageTabVisible = false;
            TabSettingsViewModel = SelectedTab as TabSettingsViewModel;
        }
    }

    private void ResizeTabByHeight(ImageTabViewModel imageTabViewModel)
    {
        imageTabViewModel.ResizeImageByHeight(ControlSize.Height);
    }


    private void ResizeTabByWidth(ImageTabViewModel imageTabViewModel)
    {
        imageTabViewModel.ResizeImageByWidth(ControlSize.Width);
    }

    #endregion

    #region Public Methods

    private async Task OpenFile(OpenFileCommand command)
    {
        var storageProvider = App.GetTopLevel()?.StorageProvider;
        if (storageProvider is null) return;
        var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open File",
            AllowMultiple = false,
            FileTypeFilter = new[] {FilePickerFileTypes.ImageAll}
        });

        if (result.Count is 0) return;
        command.Path = result[0].Path.LocalPath;
        AddImageTab(command);
    }

    public void SelectionChanged(SelectionChangedEventArgs e)
    {
        if (SelectedTab is ImageTabViewModel imageTabViewModel)
        {
            IsImageTabVisible = true;
            IsSettingsTabVisible = false;
            ImageTabViewModel = imageTabViewModel;
            if (ImageTabViewModel.IsDefaultZoom)
            {
                // Only use default zoom on image first load.
                // Since SelectionChanged is called after the image is loaded.
                // We can set the flag here.
                ImageTabViewModel.IsDefaultZoom = false;
            }
            else
            {
                MessageBus.Current.SendMessage(new SetZoomCommand(ImageTabViewModel.Matrix));
            }
        }
        else
        {
            IsImageTabVisible = false;
            IsSettingsTabVisible = true;
        }
    }

    public void FitToWidthAndResetZoom()
    {
        if (ImageTabViewModel is null) return;
        ResizeTabByWidth(ImageTabViewModel);
        MessageBus.Current.SendMessage(new ResetZoomCommand());
    }

    public void FitToHeightAndResetZoom()
    {
        if (ImageTabViewModel is null) return;
        ResizeTabByHeight(ImageTabViewModel);
        MessageBus.Current.SendMessage(new ResetZoomCommand());
    }

    private void AddImageTab(ICommandMessage command)
    {
        var imageTabViewModel = ImageTabViewModel.CreateImageTabFromCommand(command);
        if (imageTabViewModel is null) return;
        if (!Preferences.Instance.IsDuplicateTabsAllowed && OpenTabs.Any(x => x.Id == imageTabViewModel.Id)) return;

        AddTab(imageTabViewModel);
    }

    public void OpenSettingsTab()
    {
        if (IsSettingsTabOpen) return;
        AddTab(new TabSettingsViewModel());
    }

    public void CycleTab(bool reverse, bool isCtrlKey)
    {
        if (OpenTabs.Count is 1) return;
        int newIndex;
        // Calculate the index of the next tab
        if (reverse)
        {
            // Going backwards 
            newIndex = SelectedTabIndex is 0 ? OpenTabs.Count - 1 : SelectedTabIndex - 1;
        }
        else
        {
            // Going forward
            newIndex = (SelectedTabIndex + 1) % OpenTabs.Count;
        }

        if (!isCtrlKey)
        {
            if (!Preferences.Instance.IsSettingsTabCycled)
            {
                if (OpenTabs[newIndex] is TabSettingsViewModel)
                {
                    if (reverse)
                    {
                        newIndex = newIndex is 0 ? OpenTabs.Count - 1 : newIndex - 1;
                    }
                    else
                    {
                        newIndex++;
                    }
                }
            }
        }


        SelectedTabIndex = newIndex;
    }

    public void MoveTab(TabViewModelBase from, TabViewModelBase to)
    {
        var fromIdx = OpenTabs.IndexOf(from);
        var toIdx = OpenTabs.IndexOf(to);
        OpenTabs.Move(fromIdx, toIdx);
        SelectedTab = from;
    }

    #endregion
}