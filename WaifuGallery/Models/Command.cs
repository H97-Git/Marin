namespace WaifuGallery.Models;

public class Command(
    CommandType type,
    string[]? imagesInPath = null,
    string? path = null,
    int index = 0,
    string message = "")
{
    public CommandType Type { get; } = type;
    public int Index { get; } = index;
    public string? Path { get; } = path;
    public string[]? ImagesInPath { get; } = imagesInPath;
    public string? Message { get; } = message;
}

public enum CommandType
{
    ChangePath,
    ClosePreview,
    Copy,
    Cut,
    Paste,
    Delete,
    Exit,
    FitToHeight,
    FitToWidth,
    Move,
    OpenFile,
    OpenFolderInNewTab,
    OpenImageInNewTab,
    Rename,
    SendMessageToStatusBar,
    StartPreview,
    ToggleFileExplorer,
    ToggleFileExplorerScrollBar,
    ToggleFileExplorerVisibility,
    ToggleFullScreen,
    None,
}