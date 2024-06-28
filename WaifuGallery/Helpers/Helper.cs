using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Media.Imaging;
using WaifuGallery.ViewModels.FileExplorer;

namespace WaifuGallery.Helpers;

public abstract class Helper
{
    #region Private Methods

    /// <summary>
    /// Get all images in path and sort them in natural order: 1, 2, 3, 10, 11, 12
    /// </summary>
    /// <param name="di">The directory info</param>
    /// <returns>An array of FileInfo.FullName</returns>
    private static string[] GetAllImagesInPath(DirectoryInfo? di)
    {
        if (di is not null)
            return di.GetFiles()
                .Where(fileInfo => ImageFileExtensions.Contains(fileInfo.Extension.ToLower()))
                .OrderBy(fileInfo => fileInfo.Name, new NaturalSortComparer())
                .Select(fileInfo => fileInfo.FullName)
                .ToArray();
        return [];
    }

    #endregion

    #region Public Members

    public static readonly string[] ArchiveFileExtensions = [".zip", ".rar", ".7z"];
    public static readonly string[] ImageFileExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp"];
    public static readonly string[] VideoFileExtensions = [".mp4", ".mkv", ".avi", ".mov", ".wmv", ".webm", ".flv"];

    public static readonly string[] AllFileExtensions =
        ArchiveFileExtensions.Concat(ImageFileExtensions).Concat(VideoFileExtensions).ToArray();

    public static Size GetScaledSizeByWidth(Bitmap image, int targetWidth)
    {
        var correspondingHeight = targetWidth / image.Size.AspectRatio;
        return new Size(targetWidth, correspondingHeight);
    }

    public static Size GetScaledSizeByHeight(Bitmap image, int targetHeight)
    {
        var correspondingWidth = targetHeight * image.Size.AspectRatio;
        return new Size(correspondingWidth, targetHeight);
    }

    /// <summary>
    /// Use a string path to get all images
    /// </summary>
    /// <param name="path">Path</param>
    /// <returns>An array of FileInfo.FullName</returns>
    public static string[] GetAllImagesInPath(string path)
    {
        var fileAttributes = File.GetAttributes(path);
        var directoryInfo = fileAttributes.HasFlag(FileAttributes.Directory)
            ? new DirectoryInfo(path)
            : Directory.GetParent(path);

        return GetAllImagesInPath(directoryInfo);
    }

    /// <summary>
    /// Use a FileViewModel to get all images
    /// </summary>
    /// <param name="fileViewModel">FileViewModel with the path</param>
    /// <returns>An array of FileInfo.FullName</returns>
    public static string[] GetAllImagesInPath(FileViewModel fileViewModel)
    {
        // if it's an image, use the parent directory else use the full path
        var directoryInfo = fileViewModel.IsImage
            ? new DirectoryInfo(fileViewModel.ParentPath ?? Directory.GetDirectoryRoot(fileViewModel.FullPath))
            : new DirectoryInfo(fileViewModel.FullPath);
        return GetAllImagesInPath(directoryInfo);
    }

    public static long GetDirectorySizeInByte(FileSystemInfo fileSystemInfo)
    {
        var files = Directory.GetFiles(fileSystemInfo.FullName, "*.*", SearchOption.AllDirectories);
        return files.Select(file => new FileInfo(file)).Select(fileInfo => fileInfo.Length).Sum();
    }

    public static bool IsDirectoryEmpty(FileSystemInfo fileSystemInfo)
    {
        return !Directory.EnumerateFileSystemEntries(fileSystemInfo.FullName).Any();
    }

    public static void CheckDirAndCreate(string path)
    {
        var di = new DirectoryInfo(path);
        if (!di.Exists)
        {
            di.Create();
        }
    }

    #endregion
}