using Avalonia;

namespace WaifuGallery.Commands;

public interface ITabCommand : ICommandMessage;

public class OpenFileCommand : ITabCommand
{
    public string? Path { get; set; }
}

public class OpenSettingsTabCommand : ITabCommand;

public class OpenInNewTabCommand(int index, string[] imagesInPath) : IFileManagerCommand
{
    public int Index { get; } = index;
    public string[] ImagesInPath { get; } = imagesInPath;
}

public class FitToHeightCommand : ITabCommand;

public class FitToWidthCommand : ITabCommand;

public class SetZoomCommand(Matrix matrix) : ITabCommand
{
    public Matrix Matrix { get; } = matrix;
}

public class ResetZoomCommand : ITabCommand;