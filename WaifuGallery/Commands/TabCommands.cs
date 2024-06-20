namespace WaifuGallery.Commands;

public interface ITabCommand : ICommandMessage
{
}

public class OpenFileCommand : ITabCommand
{
    public string? Path { get; set; }
}

public class OpenInNewTabCommand(int index, string[] imagesInPath) : IFileExplorerCommand
{
    public int Index { get; } = index;
    public string[] ImagesInPath { get; } = imagesInPath;
}

public class FitToHeightCommand : ITabCommand
{
}

public class FitToWidthCommand : ITabCommand
{
}

public class ResetZoomCommand : ITabCommand
{
}