﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Marin.UI.Commands;
using Marin.UI.Models;
using ReactiveUI;

namespace Marin.UI.ViewModels;

/// <summary>
/// Menu Items are bound to commands in the Axaml.
/// Each command is bound to a single method in this class.
/// This single method send a message to the view (with a string parameter).
/// Since we need to make get/set/call MainWindow members or other controls across the solution.
/// </summary>
public class MenuBarViewModel : ViewModelBase
{
    #region Private Members

    private bool _isMenuOpen;
    private bool _isMenuBarVisible = true;
    private double _toggleFileManagerIconAngle = 0;
    private bool _isDebugMenuVisible = false;
    private List<MenuItem> _sessions = [];

    #endregion

    #region CTOR

    public MenuBarViewModel()
    {
#if DEBUG
        IsDebugMenuVisible = true;
#endif

        var dir = new DirectoryInfo(Settings.SessionsPath);
        var files = dir.GetFiles().Where(x => x.Extension == ".json");
        foreach (var file in files)
        {
            if (file.Name == "Last.json") continue;
            var name = file.Name.Split('.')[0];
            Sessions.Add(new MenuItem()
            {
                Header = name,
                Command = LoadSessionCommand,
                CommandParameter = name
            });
        }
    }

    #endregion

    #region Public Properties

    public bool IsMenuOpen
    {
        get => _isMenuOpen;
        set => this.RaiseAndSetIfChanged(ref _isMenuOpen, value);
    }

    public bool IsMenuBarVisible
    {
        get => _isMenuBarVisible;
        set => this.RaiseAndSetIfChanged(ref _isMenuBarVisible, value);
    }

    public bool IsDebugMenuVisible
    {
        get => _isDebugMenuVisible;
        set => this.RaiseAndSetIfChanged(ref _isDebugMenuVisible, value);
    }

    public double ToggleFileManagerIconAngle
    {
        get => _toggleFileManagerIconAngle;
        set => this.RaiseAndSetIfChanged(ref _toggleFileManagerIconAngle, value);
    }

    public List<MenuItem> Sessions
    {
        get => _sessions;
        set => this.RaiseAndSetIfChanged(ref _sessions, value);
    }

    #endregion

    #region Public Commands

    public ICommand Exit =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new ExitCommand()); });

    public ICommand CloseAllTabsCommand =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new CloseAllTabsCommand()); });

    public ICommand FitToHeightCommand =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new FitToHeightCommand()); });

    public ICommand FitToWidthCommand =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new FitToWidthCommand()); });

    public ICommand RotateClockwiseCommand =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new RotateClockwiseCommand()); });

    public ICommand RotateAntiClockwiseCommand =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new RotateAntiClockwiseCommand()); });

    public ICommand OpenFileCommand =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new OpenFileCommand()); });

    public ICommand OpenSettingsTabCommand =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new OpenSettingsTabCommand()); });

    public ICommand ToggleFileManagerCommand =>
        ReactiveCommand.Create(() =>
        {
            ToggleFileManagerIconAngle = ToggleFileManagerIconAngle == 0 ? 180 : 0;
            MessageBus.Current.SendMessage(new ToggleFileManagerCommand());
        });

    public ICommand LoadSessionCommand =>
        ReactiveCommand.Create<string>(x => { MessageBus.Current.SendMessage(new LoadSessionCommand(x)); });

    public ICommand SaveSessionCommand =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new SaveSessionCommand()); });

    public ICommand ToggleFileManagerVisibilityCommand =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new ToggleFileManagerVisibilityCommand()); });

    public ICommand ToggleFullScreenCommand =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new ToggleFullScreenCommand()); });

    public ICommand ClearCache =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new ClearCacheCommand()); });

    public ICommand GoToOxfordPet =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new GoToOxfordPet()); });

    #endregion
}