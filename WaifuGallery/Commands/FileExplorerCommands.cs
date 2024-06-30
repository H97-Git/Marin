namespace WaifuGallery.Commands;

public interface IFileExplorerCommand : ICommandMessage;

public class RefreshFileExplorerCommand : IFileExplorerCommand;

public class ChangePathCommand(string path) : IFileExplorerCommand
{
    public string Path { get; } = path;
}

public class StartPreviewCommand(string path) : IFileExplorerCommand
{
    public string Path { get; } = path;
}

public class ToggleFileExplorerCommand : ICommandMessage;

public class ToggleFileExplorerVisibilityCommand : IFileExplorerCommand;