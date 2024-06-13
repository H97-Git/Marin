using ReactiveUI;

namespace WaifuGallery.ViewModels;

public class StatusBarViewModel : ViewModelBase
{
    #region Priate Members

    private string _message = "Welcome to WaifuGallery!";
    private bool _isStatusBarVisible = true;

    #endregion

    #region Public Properties

    public string Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message, $"Information: {value}");
    }

    public bool IsStatusBarVisible
    {
        get => _isStatusBarVisible;
        set => this.RaiseAndSetIfChanged(ref _isStatusBarVisible, value);
    }

    #endregion
}