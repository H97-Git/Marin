using System;
using System.IO;
using WaifuGallery.ViewModels.FileManager;

namespace WaifuGallery.Helpers;

public abstract class PathHelper
{
    private static PathType IsFile(string path) => File.Exists(path) ? PathType.File :
        Directory.Exists(path) ? PathType.Directory : PathType.None;

    /// <summary>
    /// Use a string path to get all images
    /// </summary>
    /// <param name="path">Path</param>
    /// <param name="depth">Path</param>
    /// <returns>An array of FileInfo.FullName</returns>
    public static string[] GetAllImages(string path, int depth = 0)
    {
        var directoryInfo = IsFile(path) switch
        {
            PathType.File => Directory.GetParent(path),
            PathType.Directory => new DirectoryInfo(path),
            _ => throw new ArgumentOutOfRangeException()
        };

        return DirectoryHelper.GetAllImagesInPath(directoryInfo, depth);
    }

    /// <summary>
    /// Use a FileViewModel to get all images
    /// </summary>
    /// <param name="fileViewModel">FileViewModel with the path</param>
    /// <param name="depth"></param>
    /// <returns>An array of FileInfo.FullName</returns>
    public static string[] GetAllImages(FileViewModel fileViewModel, int depth = 0)
    {
        // if it's an image use the parent directory else use the full path
        var directoryInfo = fileViewModel.IsImage
            ? new DirectoryInfo(fileViewModel.ParentPath ?? Directory.GetDirectoryRoot(fileViewModel.FullPath))
            : new DirectoryInfo(fileViewModel.FullPath);
        return DirectoryHelper.GetAllImagesInPath(directoryInfo, depth);
    }
}