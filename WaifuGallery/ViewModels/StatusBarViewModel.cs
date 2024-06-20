using System;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using WaifuGallery.Commands;


namespace WaifuGallery.ViewModels;

public class StatusBarViewModel : ViewModelBase
{
    #region Priate Members

    private InfoBarSeverity _severity = InfoBarSeverity.Informational;
    private string _title = "Information";
    private string _message = "Welcome to WaifuGallery!";
    private IBrush _backgroundColor = new SolidColorBrush(Colors.Transparent);
    private bool _isStatusBarVisible = true;

    #endregion

    #region CTOR

    public StatusBarViewModel()
    {
        MessageBus.Current.Listen<SendMessageToStatusBarCommand>()
            .Subscribe(SetMessage);

        this.WhenAnyValue(x => x.Severity).Subscribe(x =>
        {
            BackgroundColor = x switch
            {
                InfoBarSeverity.Informational => new SolidColorBrush(Colors.LightSkyBlue),
                InfoBarSeverity.Success => new SolidColorBrush(Colors.LimeGreen),
                InfoBarSeverity.Warning => new SolidColorBrush(Colors.DarkOrange),
                InfoBarSeverity.Error => new SolidColorBrush(Colors.OrangeRed),
                _ => throw new ArgumentOutOfRangeException(nameof(x), x, null)
            };
        });
    }

    private void SetMessage(SendMessageToStatusBarCommand command)
    {
        Severity = command.Severity;
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

    public IBrush BackgroundColor
    {
        get => _backgroundColor;
        set => this.RaiseAndSetIfChanged(ref _backgroundColor, value);
    }

    public bool IsStatusBarVisible
    {
        get => _isStatusBarVisible;
        set => this.RaiseAndSetIfChanged(ref _isStatusBarVisible, value);
    }

    #endregion
}