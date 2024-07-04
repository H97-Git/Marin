using FluentAvalonia.UI.Controls;

namespace WaifuGallery.Commands;

public interface ICommandMessage;

public class ExitCommand : ICommandMessage;

public class SendMessageToStatusBarCommand(InfoBarSeverity severity, string message) : ICommandMessage
{
    public InfoBarSeverity Severity { get; } = severity;

    // public string Title { get; } = title;
    public string Message { get; } = message;
}

public class ToggleFullScreenCommand : ICommandMessage;

public class ClearCacheCommand : ICommandMessage;