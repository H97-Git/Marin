using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using DynamicData;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using WaifuGallery.Commands;
using WaifuGallery.Helpers;
using WaifuGallery.Models;

namespace WaifuGallery.ViewModels.Tabs;

public class TabsViewModel : ViewModelBase
{
    #region Private Fields

    private ObservableCollection<TabViewModelBase> _openTabs = [];
    private TabViewModelBase? _selectedTab;
    private ImageTabViewModel? _currentImageTabViewModel;
    private PreferencesTabViewModel? _tabSettingsViewModel;
    private int _selectedTabIndex;
    private bool _isSettingsTabVisible = true;
    private bool _isImageTabVisible;
    private bool _isTabHeadersVisible = true;
    private bool IsSettingsTabOpen => OpenTabs.Any(x => x is PreferencesTabViewModel);

    #endregion

    #region Private Methods

    private void SendMessageToStatusBar(string message)
    {
        SendMessageToStatusBar(InfoBarSeverity.Informational, message);
    }

    private void CloseTab()
    {
        switch (SelectedTab)
        {
            case null:
            case PreferencesTabViewModel when !Settings.Instance.TabsPreference.IsTabSettingsClosable:
                return;
            default:
                OpenTabs.Remove(SelectedTab);
                break;
        }
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
            TabSettingsViewModel = SelectedTab as PreferencesTabViewModel;
        }
    }

    private void LoadSession(string sessionName = "Last")
    {
        var sessionPath = Path.Combine(Settings.SessionsPath, $"{sessionName}.json");
        if (!File.Exists(sessionPath)) return;
        var session = JsonSerializer.Deserialize<string[]>(File.ReadAllText(sessionPath));
        if (session is null) return;
        OpenTabs.Clear();
        foreach (var path in session)
        {
            var imagesInPath = Helper.GetAllImagesInPath(path);
            if (imagesInPath is {Length: 0}) continue;
            var index = imagesInPath.IndexOf(path);
            AddImageTab(new OpenInNewTabCommand(index, imagesInPath));
        }

        // TODO: Fix this ugly hack.
        if (SelectedTab is ImageTabViewModel imageTabViewModel)
        {
            imageTabViewModel.LoadNextImage();
            imageTabViewModel.LoadPreviousImage();
        }
    }

    #endregion

    #region CTOR

    public TabsViewModel()
    {
        this.WhenAnyValue(x => x.OpenTabs.Count).Subscribe(_ =>
        {
            if (OpenTabs.Count is 0)
            {
                SelectedTab = null;
                TabSettingsViewModel = null;
                ImageTabViewModel = null;
                IsSettingsTabVisible = false;
                IsImageTabVisible = false;
                return;
            }

            SelectedTab = OpenTabs.Count is 1 ? OpenTabs.First() : OpenTabs.Last();
            if (SelectedTab is PreferencesTabViewModel)
            {
                IsSettingsTabVisible = true;
                IsImageTabVisible = false;
                TabSettingsViewModel = SelectedTab as PreferencesTabViewModel;
            }
            else if (SelectedTab is ImageTabViewModel)
            {
                IsSettingsTabVisible = false;
                IsImageTabVisible = true;
                ImageTabViewModel = SelectedTab as ImageTabViewModel;
            }
        });

        if (Settings.Instance.TabsPreference.OpenPreferencesOnStartup)
        {
            OpenSettingsTab();
        }

        if (Settings.Instance.TabsPreference.LoadLastSessionOnStartUp)
        {
            Dispatcher.UIThread.Post(() => LoadSession());
        }

        MessageBus.Current.Listen<OpenFileCommand>().Subscribe(OpenFile);
        MessageBus.Current.Listen<OpenInNewTabCommand>().Subscribe(AddImageTab);
        MessageBus.Current.Listen<FitToHeightCommand>().Subscribe(_ => FitToHeightAndResetZoom());
        MessageBus.Current.Listen<FitToWidthCommand>().Subscribe(_ => FitToWidthAndResetZoom());
        MessageBus.Current.Listen<OpenSettingsTabCommand>().Subscribe(_ => OpenSettingsTab());
        MessageBus.Current.Listen<RotateClockwiseCommand>().Subscribe(_ => RotateAndResetZoom());
        MessageBus.Current.Listen<RotateAntiClockwiseCommand>().Subscribe(_ => RotateAndResetZoom(false));

        MessageBus.Current.Listen<LoadSessionCommand>().Subscribe(_ => LoadSession());
        MessageBus.Current.Listen<SaveSessionCommand>().Subscribe(_ => Settings.SaveSession());
    }

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

    public PreferencesTabViewModel? TabSettingsViewModel
    {
        get => _tabSettingsViewModel;
        set => this.RaiseAndSetIfChanged(ref _tabSettingsViewModel, value);
    }

    public Size ControlSize { get; set; }

    public ICommand CloseTabCommand =>
        ReactiveCommand.Create(CloseTab);

    #endregion

    #region Public Methods

    private async void OpenFile(OpenFileCommand command)
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
        ImageTabViewModel.ResizeImageByWidth(ControlSize.Width);
        MessageBus.Current.SendMessage(new ResetZoomCommand());
    }

    public void FitToHeightAndResetZoom()
    {
        if (ImageTabViewModel is null) return;
        ImageTabViewModel.ResizeImageByHeight(ControlSize.Height);
        MessageBus.Current.SendMessage(new ResetZoomCommand());
    }

    private void RotateAndResetZoom(bool clockwise = true)
    {
        if (ImageTabViewModel is null) return;
        ImageTabViewModel.RotateImage(clockwise);
        MessageBus.Current.SendMessage(new ResetZoomCommand());
    }

    private void AddImageTab(ICommandMessage command)
    {
        var imageTabViewModel = ImageTabViewModel.CreateImageTabFromCommand(command);
        if (imageTabViewModel is null) return;
        if (!Settings.Instance.TabsPreference.IsDuplicateTabsAllowed &&
            OpenTabs.Any(x => x.Id == imageTabViewModel.Id)) return;

        AddTab(imageTabViewModel);
    }

    public void OpenSettingsTab()
    {
        if (IsSettingsTabOpen) return;
        AddTab(new PreferencesTabViewModel());
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
            if (!Settings.Instance.TabsPreference.IsSettingsTabCycled)
            {
                if (OpenTabs[newIndex] is PreferencesTabViewModel)
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