using Avalonia;

namespace WaifuGallery.Commands;

public interface ITabCommand : ICommandMessage;

public class CloseAllTabsCommand : ITabCommand;
public class FitToHeightCommand : ITabCommand;

public class FitToWidthCommand : ITabCommand;

public class OpenFileCommand : ITabCommand
{
    public string? Path { get; set; }
}

public class OpenInNewTabCommand(int index, string[] imagesInPath) : IFileManagerCommand
{
    public int Index { get; } = index;
    public string[] ImagesInPath { get; } = imagesInPath;
}

public class OpenSettingsTabCommand : ITabCommand;

public class ResetZoomCommand : ITabCommand;

public class RotateClockwiseCommand : ITabCommand;

public class RotateAntiClockwiseCommand : ITabCommand;

public class SetZoomCommand(Matrix matrix) : ITabCommand
{
    public Matrix Matrix { get; } = matrix;
}