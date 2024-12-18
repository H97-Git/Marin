﻿using FluentAvalonia.UI.Controls;

namespace Marin.UI.Commands;

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

public class GoToOxfordPet : ICommandMessage;

public class LoadSessionCommand(string sessionName) : ICommandMessage
{
    public string SessionName { get; } = sessionName;
}

public class SaveSessionCommand : ICommandMessage;