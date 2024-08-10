using System.Collections.ObjectModel;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using Serilog;
using WaifuGallery.Commands;

namespace WaifuGallery.ViewModels;

public class ViewModelBase : ReactiveObject
{
    private ObservableCollection<string> _errors = new();

    public ObservableCollection<string> Errors
    {
        get => _errors;
        set => this.RaiseAndSetIfChanged(ref _errors, value);
    }

    protected static void SendMessageToStatusBar(InfoBarSeverity severity, string message)
    {
        Log.Debug("SendMessageToStatusBar: {Severity}: {Message}", severity, message);
        MessageBus.Current.SendMessage(new SendMessageToStatusBarCommand(severity, message));
    }
}