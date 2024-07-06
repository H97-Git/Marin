using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using FluentAvalonia.UI.Controls;
using ImageMagick;
using NaturalSort.Extension;
using ReactiveUI;
using SharpCompress.Archives;
using SharpCompress.Common;
using WaifuGallery.Commands;
using WaifuGallery.Models;
using WaifuGallery.ViewModels;
using WaifuGallery.ViewModels.FileExplorer;

namespace WaifuGallery.Helpers;

public abstract class Helper
{
    #region Private Methods

    private static string GetAllImagesInPathKey(string fullName, int depth)
    {
        var hash = fullName.GetHashCode();
        return $"AllImagesInPath_{hash}_{depth}";
    }

    private static string GetSizeKey(FileSystemInfo fileSystemInfo)
    {
        var hash = fileSystemInfo.FullName.GetHashCode();
        var name = fileSystemInfo.Name;
        return $"ImagesInPath_{hash}_{name}";
    }

    /// <summary>
    /// Get all images in path and sort them in natural order: 1, 2, 3, 10, 11, 12
    /// </summary>
    /// <param name="di">The directory info</param>
    /// <param name="depth">The directory info</param>
    /// <returns>An array of FileInfo.FullName</returns>
    private static string[] GetAllImagesInPath(DirectoryInfo? di, int depth)
    {
        if (di is null || depth < 0)
            return Array.Empty<string>();

        var key = GetAllImagesInPathKey(di.FullName, depth);
        var allImagesInPath = MemoryCacheService.Get<string[]>(key);
        if (allImagesInPath is not null) return allImagesInPath;
        allImagesInPath = GetImagesRecursive(di, depth).ToArray();
        MemoryCacheService.AddOrUpdate(key, allImagesInPath, 5);
        return allImagesInPath;
    }

    private static IEnumerable<string> GetImagesRecursive(DirectoryInfo di, int depth)
    {
        // Get images in the current directory
        var images = di.GetFiles()
            .Where(fileInfo => ImageFileExtensions.Contains(fileInfo.Extension.ToLower()))
            .OrderBy(fileInfo => fileInfo.Name, StringComparison.OrdinalIgnoreCase.WithNaturalSort())
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
    /// <param name="depth">Path</param>
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

    public static bool ThumbnailExists(FileInfo fileInfo, out string thumbnailPath)
    {
        var directoryName = fileInfo.Directory is null ? "Root" : fileInfo.Directory.Name;
        var currentPathInCache = Path.Combine(Settings.ThumbnailsPath, directoryName);
        Directory.CreateDirectory(currentPathInCache);
        thumbnailPath = GetAllImagesInPath(currentPathInCache)
            .FirstOrDefault(x => Path.GetFileName(x) == fileInfo.Name) ?? currentPathInCache;
        return File.Exists(thumbnailPath);
    }

    public static async Task<Bitmap> GenerateBitmapThumb(FileInfo sourceFileInfo, FileInfo outputFileInfo)
    {
        using var image = new MagickImage(sourceFileInfo);
        image.Resize(new MagickGeometry(100, 100)
        {
            IgnoreAspectRatio = false
        });
        await image.WriteAsync(outputFileInfo);
        return new Bitmap(outputFileInfo.FullName);
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
        if (fileSystemInfo is DirectoryInfo directoryInfo)
        {
            if (directoryInfo.Root.FullName == directoryInfo.FullName) return 0;
        }

        try
        {
            var sizeInCache = MemoryCacheService.Get<long>(GetSizeKey(fileSystemInfo));
            if (sizeInCache is not 0) return sizeInCache;
            var files = Directory.GetFiles(fileSystemInfo.FullName, "*.*", SearchOption.AllDirectories);
            var size = files.Select(file => new FileInfo(file)).Select(fileInfo => fileInfo.Length).Sum();
            MemoryCacheService.AddOrUpdate(GetSizeKey(fileSystemInfo), size);
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

    public static void ExtractDirectory(string path)
    {
        using var archive = ArchiveFactory.Open(path);
        foreach (var entry in archive.Entries)
        {
            entry.WriteToDirectory(Path.Combine(path, "Extracted"), new ExtractionOptions()
            {
                ExtractFullPath = true,
                Overwrite = true
            });
        }
    }

    public static void ClearThumbnailsCache()
    {
        var path = Settings.ThumbnailsPath;
        if (!Directory.Exists(path)) return;
        var command = new SendMessageToStatusBarCommand(InfoBarSeverity.Success,
            "Thumbnails cache cleared successfully!");
        try
        {
            Directory.Delete(path, true);
        }
        catch (Exception e)
        {
            command = new SendMessageToStatusBarCommand(InfoBarSeverity.Error,
                "Failed to clear thumbnails cache: " + e.Message);
        }
        finally
        {
            MessageBus.Current.SendMessage(command);
        }
    }
}

public enum PathType
{
    File,
    Directory,
    None
}