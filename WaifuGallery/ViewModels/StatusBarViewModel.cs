﻿using System;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using WaifuGallery.Commands;
using WaifuGallery.Models;
using Timer = System.Timers.Timer;

namespace WaifuGallery.ViewModels;

public class StatusBarViewModel : ViewModelBase
{
    #region Priate Fields

    private IBrush _backgroundColor = new SolidColorBrush(Colors.Transparent);
    private IBrush _foregroundColor = new SolidColorBrush(Colors.White);
    private InfoBarSeverity _severity = InfoBarSeverity.Informational;
    private bool _isStatusBarVisible = true;
    private string _message = "Welcome to WaifuGallery!";
    private string _title = "Information";
    private int _countDuplicates;
    private readonly Timer _timer;

    #endregion

    #region Private Methods

    private void SetMessage(SendMessageToStatusBarCommand command)
    {
        IsStatusBarVisible = true;
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

        if (!Settings.Instance.AutoHideStatusBar) return;
        _timer.Stop();
        _timer.Start();
    }

    #endregion

    #region CTOR

    public StatusBarViewModel()
    {
        _timer = new Timer(3000);
        _timer.Elapsed += (_, _) => IsStatusBarVisible = false;
        _timer.AutoReset = false;
        MessageBus.Current.Listen<SendMessageToStatusBarCommand>()
            .Subscribe(SetMessage);

        this.WhenAnyValue(x => x.Severity).Subscribe(x =>
        {
            BackgroundColor = x switch
            {
                InfoBarSeverity.Error => new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                InfoBarSeverity.Informational => new SolidColorBrush(Color.FromRgb(52, 152, 219)),
                InfoBarSeverity.Success => new SolidColorBrush(Color.FromRgb(46, 204, 113)),
                InfoBarSeverity.Warning => new SolidColorBrush(Color.FromRgb(230, 126, 34)),
                _ => throw new ArgumentOutOfRangeException(nameof(x), x, null)
            };

            Title = x switch
            {
                InfoBarSeverity.Error => "Error",
                InfoBarSeverity.Informational => "Information",
                InfoBarSeverity.Success => "Success",
                InfoBarSeverity.Warning => "Warning",
                _ => throw new ArgumentOutOfRangeException(nameof(x), x, null)
            };
        });
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

    public IBrush ForegroundColor
    {
        get => _foregroundColor;
        set => this.RaiseAndSetIfChanged(ref _foregroundColor, value);
    }

    public bool IsStatusBarVisible
    {
        get => _isStatusBarVisible;
        set => this.RaiseAndSetIfChanged(ref _isStatusBarVisible, value);
    }

    #endregion
}