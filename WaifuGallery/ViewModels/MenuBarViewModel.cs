using System;
using System.Windows.Input;
using ReactiveUI;
using WaifuGallery.Models;

namespace WaifuGallery.ViewModels;

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
    private bool _isMenuVisible = true;

    private Action SendCommandToMainView(Command command) => () => { OnSendCommandToMainView?.Invoke(this, command); };

    #endregion

    #region Public Properties

    public bool IsMenuOpen
    {
        get => _isMenuOpen;
        set => this.RaiseAndSetIfChanged(ref _isMenuOpen, value);
    }

    public bool IsMenuVisible
    {
        get => _isMenuVisible;
        set => this.RaiseAndSetIfChanged(ref _isMenuVisible, value);
    }

    #endregion

    #region Public Events

    public event EventHandler<Command>? OnSendCommandToMainView;

    #endregion

    #region Public Commands

    public ICommand Exit =>
        ReactiveCommand.Create(SendCommandToMainView(new Command(CommandType.Exit)));

    public ICommand FitToHeightCommand =>
        ReactiveCommand.Create(SendCommandToMainView(new Command(CommandType.FitToHeight)));

    public ICommand FitToWidthCommand =>
        ReactiveCommand.Create(SendCommandToMainView(new Command(CommandType.FitToWidth)));

    public ICommand OpenFileCommand =>
        ReactiveCommand.Create(SendCommandToMainView(new Command(CommandType.OpenFile)));

    public ICommand ToggleFileExplorerCommand =>
        ReactiveCommand.Create(SendCommandToMainView(new Command(CommandType.ToggleFileExplorer)));

    public ICommand ToggleFileExplorerVisibilityCommand =>
        ReactiveCommand.Create(SendCommandToMainView(new Command(CommandType.ToggleFileExplorerVisibility)));

    public ICommand ToggleFullScreenCommand =>
        ReactiveCommand.Create(SendCommandToMainView(new Command(CommandType.ToggleFullScreen)));

    #endregion
}