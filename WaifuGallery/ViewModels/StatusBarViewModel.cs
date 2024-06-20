using System;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using WaifuGallery.Commands;


namespace WaifuGallery.ViewModels;

public class StatusBarViewModel : ViewModelBase
{
    #region Priate Members

    private string _title = "Information";
    private string _message = "Welcome to WaifuGallery!";
    private bool _isStatusBarVisible = true;
    private InfoBarSeverity _severity = InfoBarSeverity.Informational;
    
    #endregion

    #region CTOR

    public StatusBarViewModel()
    {
        MessageBus.Current.Listen<SendMessageToStatusBarCommand>()
            .Subscribe(SetMessage);
    }

    private void SetMessage(SendMessageToStatusBarCommand command)
    {
        Title = command.Title;
        Message = command.Message;
        IsStatusBarVisible = true;
    }

    #endregion

    #region Public Properties

    public InfoBarSeverity Severity
    {
        get => _severity;
        set => this.RaiseAndSetIfChanged(ref _severity, value);
    }
    
    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    public string Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message, value);
    }

    public bool IsStatusBarVisible
    {
        get => _isStatusBarVisible;
        set => this.RaiseAndSetIfChanged(ref _isStatusBarVisible, value);
    }

    #endregion
}