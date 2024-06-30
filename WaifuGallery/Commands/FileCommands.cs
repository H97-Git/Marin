namespace WaifuGallery.Commands;

public interface IFileCommand : ICommandMessage
{
}

public class FileCommand(string path) : IFileCommand
{
    public string Path { get; } = path;
}

public class CopyCommand(string path) : FileCommand(path);

public class CutCommand(string path) : FileCommand(path);

public class DeleteCommand(string path) : FileCommand(path);

public class NewFolderCommand(string path) : FileCommand(path);

public class PasteCommand(string path) : FileCommand(path);

public class RenameCommand(string path, string newName) : FileCommand(path)
{
    public string NewName { get; } = newName;
}