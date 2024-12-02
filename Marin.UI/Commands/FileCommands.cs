namespace Marin.UI.Commands;

public interface IFileCommand : ICommandMessage
{
}

public class FileCommand(string path) : IFileCommand
{
    public string Path { get; set; } = path;
}

public class CopyCommand(string path) : FileCommand(path);

public class CutCommand(string path) : FileCommand(path);

public class DeleteCommand(string path) : FileCommand(path);

public class ExtractCommand(string path) : FileCommand(path);

public class NewFolderCommand(string path) : FileCommand(path);

public class OpenInBrowserCommand(string path) : FileCommand(path);

public class OpenInFileExplorerCommand(string path) : FileCommand(path);

public class PasteCommand(string path) : FileCommand(path);

public class RenameCommand(string path, string newName) : FileCommand(path)
{
    public string NewName { get; } = newName;
}