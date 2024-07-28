namespace WaifuGallery.Commands;

public interface IFileManagerCommand : ICommandMessage;
public class ChangePathCommand(string path) : IFileManagerCommand
{
    public string Path { get; } = path;
}

public class RefreshFileManagerCommand(IFileCommand fileCommand) : IFileManagerCommand
{
    public IFileCommand FileCommand { get; } = fileCommand;
}

public class SetFileManagerPositionCommand(string position) : IFileManagerCommand
{
    public string Position { get; } = position;
}

public class StartPreviewCommand(string path) : IFileManagerCommand
{
    public string Path { get; } = path;
}

public class ToggleFileManagerCommand : IFileManagerCommand;

public class ToggleFileManagerVisibilityCommand : IFileManagerCommand;