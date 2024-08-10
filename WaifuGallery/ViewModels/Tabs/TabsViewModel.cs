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
using ReactiveUI;
using Serilog;
using WaifuGallery.Commands;
using WaifuGallery.Helpers;
using WaifuGallery.Models;

namespace WaifuGallery.ViewModels.Tabs;

public class TabsViewModel : ViewModelBase
{
    #region Private Fields

    private ImageTabViewModel? _currentImageTabViewModel;
    private ObservableCollection<TabViewModelBase> _openTabs = [];
    private PreferencesTabViewModel? _tabSettingsViewModel;
    private TabViewModelBase? _selectedTab;
    private bool _isTabHeadersVisible = true;
    private bool _isImageTabVisible;
    private bool _isSettingsTabVisible = true;
    private int _selectedTabIndex;

    #endregion

    #region Private Methods

    private void CloseTab()
    {
        Log.Debug("Close selected tab");
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
        Log.Debug("Add new tab {Tab}", tab.Id);
        OpenTabs.Add(tab);
    }

    private void LoadSession(string sessionName = "Last")
    {
        Log.Debug("Load session {SessionName}", sessionName);
        var sessionPath = Path.Combine(Settings.SessionsPath, $"{sessionName}.json");
        if (!File.Exists(sessionPath)) return;
        var session = JsonSerializer.Deserialize<string[]>(File.ReadAllText(sessionPath));
        if (session is null) return;
        OpenTabs.Clear();
        foreach (var path in session)
        {
            var imagesInPath = PathHelper.GetAllImages(path);
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
                TabSettingsViewModel = null;
                ImageTabViewModel = null;
                IsSettingsTabVisible = false;
                IsImageTabVisible = false;
                SelectedTab = null;
                return;
            }

            // TODO: SelectedTab is sometimes null when it's not supposed to. Need fix
            SelectedTab = OpenTabs.Count is 1 ? OpenTabs.First() : OpenTabs.Last();
        });

        if (Settings.Instance.TabsPreference.OpenPreferencesOnStartup)
        {
            OpenSettingsTab();
        }

        if (Settings.Instance.TabsPreference.LoadLastSessionOnStartUp)
        {
            Dispatcher.UIThread.Post(() => LoadSession());
        }

        MessageBus.Current.Listen<OpenFileCommand>().Subscribe(OpenFileInNewTab);
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

    private async void OpenFileInNewTab(OpenFileCommand command)
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
        Log.Debug("Opening file in a new tab {Path}", command.Path);
        command.Path = result[0].Path.LocalPath;
        AddImageTab(command);
    }

    public void SelectionChanged(SelectionChangedEventArgs e)
    {
        if (SelectedTab is null)
            return;
        if (SelectedTab is ImageTabViewModel imageTabViewModel)
        {
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

            IsImageTabVisible = true;
        }
        else
        {
            IsImageTabVisible = false;
            TabSettingsViewModel = SelectedTab as PreferencesTabViewModel;
            IsSettingsTabVisible = true;
        }
    }

    public void FitToWidthAndResetZoom()
    {
        Log.Debug("FitToWidthAndResetZoom");
        if (ImageTabViewModel is null) return;
        ImageTabViewModel.ResizeImageByWidth(ControlSize.Width);
        MessageBus.Current.SendMessage(new ResetZoomCommand());
    }

    public void FitToHeightAndResetZoom()
    {
        Log.Debug("FitToHeightAndResetZoom");
        if (ImageTabViewModel is null) return;
        ImageTabViewModel.ResizeImageByHeight(ControlSize.Height);
        MessageBus.Current.SendMessage(new ResetZoomCommand());
    }

    private void RotateAndResetZoom(bool clockwise = true)
    {
        Log.Debug("RotateAndResetZoom");
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
        if (OpenTabs.Any(x => x is PreferencesTabViewModel)) return;
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
        Log.Debug("Move tab {From} to {To}", from.Id, to.Id);
        var fromIdx = OpenTabs.IndexOf(from);
        var toIdx = OpenTabs.IndexOf(to);
        OpenTabs.Move(fromIdx, toIdx);
        SelectedTab = from;
    }

    #endregion
}