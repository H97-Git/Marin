using System.Windows.Input;
using ReactiveUI;
using WaifuGallery.Commands;

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
    private double _toggleFileManagerIconAngle = 0;
    private bool _isDebugMenuVisible = false;

    #endregion

    #region CTOR

    public MenuBarViewModel()
    {
#if DEBUG
        IsDebugMenuVisible = true;
#endif
    }

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

    #endregion

    #region Public Commands

    public ICommand Exit =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new ExitCommand()); });

    public ICommand FitToHeightCommand =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new FitToHeightCommand()); });

    public ICommand FitToWidthCommand =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new FitToWidthCommand()); });


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


    public ICommand ToggleFileManagerVisibilityCommand =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new ToggleFileManagerVisibilityCommand()); });

    public ICommand ToggleFullScreenCommand =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new ToggleFullScreenCommand()); });

    public ICommand ClearCache =>
        ReactiveCommand.Create(() => { MessageBus.Current.SendMessage(new ClearCacheCommand()); });

    #endregion
}