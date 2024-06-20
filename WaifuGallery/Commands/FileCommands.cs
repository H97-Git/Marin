namespace WaifuGallery.Commands;

public interface IFileCommand : ICommandMessage
{
}

public class CopyCommand(string path) : IFileCommand
{
    public string Path { get; } = path;
}

public class CutCommand(string path) : IFileCommand
{
    public string Path { get; } = path;
}

public class DeleteCommand : IFileCommand
{
}

public class MoveCommand : IFileCommand
{
}

public class PasteCommand : IFileCommand
{
}

public class RenameCommand(string oldName, string newName) : IFileCommand
{
    public string OldName { get; } = oldName;
    public string NewName { get; } = newName;
}