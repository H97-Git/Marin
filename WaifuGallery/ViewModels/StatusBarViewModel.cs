using System;
using System.Threading.Tasks;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using Serilog;
using WaifuGallery.Commands;
using WaifuGallery.Models;
using Timer = System.Timers.Timer;

namespace WaifuGallery.ViewModels;

public class StatusBarViewModel : ViewModelBase
{
    #region Priate Fields

    private readonly Timer _timer;
    private IBrush _backgroundColor = new SolidColorBrush(Colors.Transparent);
    private InfoBarSeverity _severity;
    private bool _isStatusBarVisible = true;
    private bool _isStatusBarCollapsed = false;
    private int _countDuplicates;
    private string _message = "";
    private string _title = "";

    #endregion

    #region Private Methods

    private void SetMessage(SendMessageToStatusBarCommand command)
    {
        Log.Debug("Set status bar message: {Message}", command.Message);
        Severity = command.Severity;
        Message = Message.Replace($" ({_countDuplicates})", "");
        if (command.Message == Message)
        {
            _countDuplicates++;
            Message = $"{Message} ({_countDuplicates})";
        }
        else
        {
            _countDuplicates = 0;
            Message = command.Message;
        }

        IsStatusBarVisible = true;
        IsStatusBarCollapsed = false;
        if (!Settings.Instance.StatusBarPreference.AutoHideStatusBar) return;
        _timer.Stop();
        _timer.Interval = Settings.Instance.StatusBarPreference.AutoHideStatusBarDelay;
        _timer.Start();
    }

    private void SetTitleAndBackGroundColor(InfoBarSeverity severity)
    {
        BackgroundColor = severity switch
        {
            InfoBarSeverity.Error => new SolidColorBrush(Color.FromRgb(231, 76, 60)),
            InfoBarSeverity.Informational => new SolidColorBrush(Color.FromRgb(52, 152, 219)),
            InfoBarSeverity.Success => new SolidColorBrush(Color.FromRgb(46, 204, 113)),
            InfoBarSeverity.Warning => new SolidColorBrush(Color.FromRgb(230, 126, 34)),
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
        };

        Title = severity switch
        {
            InfoBarSeverity.Error => "Error",
            InfoBarSeverity.Informational => "Information",
            InfoBarSeverity.Success => "Success",
            InfoBarSeverity.Warning => "Warning",
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
        };
    }

    #endregion

    #region CTOR

    public StatusBarViewModel()
    {
        _timer = new Timer(Settings.Instance.StatusBarPreference.AutoHideStatusBarDelay);
        _timer.Elapsed += (_, _) =>
        {
            IsStatusBarCollapsed = true;
            Task.Delay(1000).ContinueWith(_ => { IsStatusBarVisible = false; });
        };
        _timer.AutoReset = false;
        MessageBus.Current.Listen<SendMessageToStatusBarCommand>().Subscribe(SetMessage);
        this.WhenAnyValue(x => x.Severity).Subscribe(SetTitleAndBackGroundColor);

        SetMessage(new SendMessageToStatusBarCommand(InfoBarSeverity.Informational, "Welcome to WaifuGallery!"));
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

    public bool IsStatusBarCollapsed
    {
        get => _isStatusBarCollapsed;
        set => this.RaiseAndSetIfChanged(ref _isStatusBarCollapsed, value);
    }

    public bool IsStatusBarVisible
    {
        get => _isStatusBarVisible;
        set => this.RaiseAndSetIfChanged(ref _isStatusBarVisible, value);
    }

    #endregion
}