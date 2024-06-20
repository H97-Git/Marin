namespace WaifuGallery.Commands;

public class ExitCommand : ICommandMessage
{
}

public class SendMessageToStatusBarCommand(string title, string message) : ICommandMessage
{
    public string Title { get; } = title;
    public string Message { get; } = message;
}

public class ToggleFullScreenCommand : ICommandMessage
{
}