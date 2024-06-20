namespace WaifuGallery.Commands;

public interface IFileExplorerCommand : ICommandMessage
{
}

public class ChangePathCommand(string path) : IFileExplorerCommand
{
    public string Path { get; } = path;
}

public class ClosePreviewCommand : IFileExplorerCommand
{
}

public class StartPreviewCommand(string[] imagesInPath) : IFileExplorerCommand
{
    public string[] ImagesInPath { get; } = imagesInPath;
}

public class ToggleFileExplorerCommand : ICommandMessage
{
}

public class ToggleFileExplorerScrollBarCommand : IFileExplorerCommand
{
}

public class ToggleFileExplorerVisibilityCommand : IFileExplorerCommand
{
}