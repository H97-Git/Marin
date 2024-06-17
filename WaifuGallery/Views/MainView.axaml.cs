using Avalonia.Controls;
using Avalonia.Input;
using WaifuGallery.ViewModels;

namespace WaifuGallery.Views;

public partial class MainView : UserControl
{
    #region Private Members

    private MainViewViewModel? MainViewModel => DataContext as MainViewViewModel;

    #endregion

    #region CTOR

    public MainView()
    {
        InitializeComponent();
        FileExplorerPanel.PointerReleased += InputElement_OnPointerReleased;
    }

    #endregion
    
    #region Private Methods

    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton is MouseButton.Middle) return;
        switch (MainViewModel)
        {
            case {FileExplorerViewModel.IsFileExplorerExpanded: false}:
            case {FileExplorerViewModel.IsFileExplorerVisible: false}:
            case {FileExplorerViewModel.IsPointerOver: true}:
                return;
            default:
                MainViewModel?.FileExplorerViewModel.ToggleFileExplorer();
                break;
        }
    }

    /// <summary>
    /// MainView_OnKeyDown is the only place (so far) where the Tab key is handled.
    /// The rest of the key events are send from the MainWindow, handled in the MainViewModel.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MainView_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key is not Key.Tab) return;
        MainViewModel?.HandleTabKeyEvent(e);
    }

    #endregion
}