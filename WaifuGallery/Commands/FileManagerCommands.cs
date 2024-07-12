namespace WaifuGallery.Commands;

public interface IFileManagerCommand : ICommandMessage;

public class RefreshFileManagerCommand(IFileCommand fileCommand) : IFileManagerCommand
{
    public IFileCommand FileCommand { get; } = fileCommand;
}

public class ChangePathCommand(string path) : IFileManagerCommand
{
    public string Path { get; } = path;
}

public class StartPreviewCommand(string path) : IFileManagerCommand
{
    public string Path { get; } = path;
}

public class ToggleFileManagerCommand : ICommandMessage;

public class ToggleFileManagerVisibilityCommand : IFileManagerCommand;