using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Media.Imaging;
using WaifuGallery.Models;
using WaifuGallery.ViewModels.FileExplorer;

namespace WaifuGallery.Helpers;

public abstract class Helper
{
    #region Private Methods

    /// <summary>
    /// Get all images in path and sort them in natural order: 1, 2, 3, 10, 11, 12
    /// </summary>
    /// <param name="di">The directory info</param>
    /// <param name="di">The directory info</param>
    /// <returns>An array of FileInfo.FullName</returns>
    private static string[] GetAllImagesInPath(DirectoryInfo? di, int depth)
    {
        if (di is null || depth < 0)
            return Array.Empty<string>();

        return GetImagesRecursive(di, depth).ToArray();
    }

    private static IEnumerable<string> GetImagesRecursive(DirectoryInfo di, int depth)
    {
        // Get images in the current directory
        var images = di.GetFiles()
            .Where(fileInfo => ImageFileExtensions.Contains(fileInfo.Extension.ToLower()))
            .OrderBy(fileInfo => fileInfo.Name, new NaturalSortComparer())
            .Select(fileInfo => fileInfo.FullName);

        foreach (var image in images)
        {
            yield return image;
        }

        // If depth is greater than 0, get images from subdirectories
        if (depth <= 0) yield break;
        foreach (var subDir in di.GetDirectories())
        {
            var subDirImages = GetImagesRecursive(subDir, depth - 1);
            foreach (var subDirImage in subDirImages)
            {
                yield return subDirImage;
            }
        }
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

    public static Size GetScaledSize(Bitmap image, int desiredSize)
    {
        var isPortrait = image.Size.Width < image.Size.Height;
        return isPortrait
            ? GetScaledSizeByHeight(image, desiredSize)
            : GetScaledSizeByWidth(image, desiredSize);
    }

    /// <summary>
    /// Use a string path to get all images
    /// </summary>
    /// <param name="path">Path</param>
    /// <returns>An array of FileInfo.FullName</returns>
    public static string[] GetAllImagesInPath(string path, int depth = 0)
    {
        var directoryInfo = IsPathFile(path) switch
        {
            PathType.File => Directory.GetParent(path),
            PathType.Directory => new DirectoryInfo(path),
            _ => throw new ArgumentOutOfRangeException()
        };

        return GetAllImagesInPath(directoryInfo, depth);
    }

    /// <summary>
    /// Use a FileViewModel to get all images
    /// </summary>
    /// <param name="fileViewModel">FileViewModel with the path</param>
    /// <returns>An array of FileInfo.FullName</returns>
    public static string[] GetAllImagesInPath(FileViewModel fileViewModel)
    {
        // if it's an image use the parent directory else use the full path
        var directoryInfo = fileViewModel.IsImage
            ? new DirectoryInfo(fileViewModel.ParentPath ?? Directory.GetDirectoryRoot(fileViewModel.FullPath))
            : new DirectoryInfo(fileViewModel.FullPath);
        return GetAllImagesInPath(directoryInfo, 0);
    }

    public static long GetDirectorySizeInByte(FileSystemInfo fileSystemInfo)
    {
        try
        {
            if (DirSizeCache.TryGetValueDirSizeCache(fileSystemInfo.FullName, out var size)) return size;
            var files = Directory.GetFiles(fileSystemInfo.FullName, "*.*", SearchOption.AllDirectories);
            size = files.Select(file => new FileInfo(file)).Select(fileInfo => fileInfo.Length).Sum();
            DirSizeCache.AddOrUpdateDirSizeCache(fileSystemInfo.FullName, size);
            return size;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public static bool IsDirectoryEmpty(FileSystemInfo fileSystemInfo)
    {
        try
        {
            return !Directory.EnumerateFileSystemEntries(fileSystemInfo.FullName).Any();
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static PathType IsPathFile(string path) => File.Exists(path) ? PathType.File :
        Directory.Exists(path) ? PathType.Directory : PathType.None;


    /// <summary>
    /// https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="recursive"></param>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public static void CopyDirectory(string source, string destination, bool recursive)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(source);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        var dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(Directory.Exists(Path.Combine(destination, dir.Name))
            ? Path.Combine(destination, dir.Name + "_copy")
            : Path.Combine(destination, dir.Name));

        // Get the files in the source directory and copy to the destination directory
        foreach (var file in dir.GetFiles())
        {
            var targetFilePath = Path.Combine(destination, file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (!recursive) return;
        foreach (var subDir in dirs)
        {
            var newDestinationDir = Path.Combine(destination, subDir.Name);
            CopyDirectory(subDir.FullName, newDestinationDir, true);
        }
    }

    #endregion
}

public enum PathType
{
    File,
    Directory,
    None
}