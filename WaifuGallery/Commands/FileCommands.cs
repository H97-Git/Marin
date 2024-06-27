namespace WaifuGallery.Commands;

public interface IFileCommand : ICommandMessage;

public class CopyCommand(string path) : IFileCommand
{
    public string Path { get; } = path;
}

public class CutCommand(string path) : IFileCommand
{
    public string Path { get; } = path;
}

public class DeleteCommand : IFileCommand;

public class PasteCommand : IFileCommand;

public class RenameCommand(string path, string newName) : IFileCommand
{
    public string Path { get; } = path;
    public string NewName { get; } = newName;
}