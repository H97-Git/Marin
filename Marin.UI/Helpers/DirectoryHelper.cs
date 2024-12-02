using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAvalonia.UI.Controls;
using Marin.UI.Commands;
using Marin.UI.Models;
using NaturalSort.Extension;
using ReactiveUI;
using Serilog;
using SharpCompress.Archives;
using SharpCompress.Common;

namespace Marin.UI.Helpers;

public abstract class DirectoryHelper
{
    /// <summary>
    /// Get all images in path and sort them in natural order: 1, 2, 3, 10, 11, 12
    /// </summary>
    /// <param name="di">The directory info</param>
    /// <param name="depth">The directory info</param>
    /// <returns>An array of FileInfo.FullName</returns>
    public static string[] GetAllImagesInPath(DirectoryInfo? di, int depth)
    {
        if (di is null || depth < 0)
            return Array.Empty<string>();

        var key = InMemoryCacheKeys.GetAllImagesInPathKey(di.FullName, depth);
        var allImagesInPath = MemoryCacheService.Get<string[]>(key);
        if (allImagesInPath is not null) return allImagesInPath;
        allImagesInPath = DirectoryHelper.GetImagesRecursive(di, depth).ToArray();
        MemoryCacheService.AddOrUpdate(key, allImagesInPath, 5);
        return allImagesInPath;
    }

    private static IEnumerable<string> GetImagesRecursive(DirectoryInfo di, int depth)
    {
        // Get images in the current directory
        var images = di.GetFiles()
            .Where(fileInfo => Extensions.Images.Contains(fileInfo.Extension.ToLower()))
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

    public static long GetSizeInByte(FileSystemInfo fileSystemInfo)
    {
        Log.Debug("Getting directory size: {Path}", fileSystemInfo.FullName);
        if (fileSystemInfo is DirectoryInfo directoryInfo)
        {
            if (directoryInfo.Root.FullName == directoryInfo.FullName) return 0;
        }

        try
        {
            var sizeInCache = MemoryCacheService.Get<long>(InMemoryCacheKeys.GetDirectorySizeKey(fileSystemInfo));
            if (sizeInCache is not 0) return sizeInCache;
            var files = Directory.GetFiles(fileSystemInfo.FullName, "*.*", SearchOption.AllDirectories);
            var size = files.Select(file => new FileInfo(file)).Select(fileInfo => fileInfo.Length).Sum();
            MemoryCacheService.AddOrUpdate(InMemoryCacheKeys.GetDirectorySizeKey(fileSystemInfo), size);
            return size;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public static bool IsEmpty(FileSystemInfo fileSystemInfo)
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

    /// <summary>
    /// https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="recursive"></param>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public static DirectoryInfo Copy(string source, string destination, bool recursive)
    {
        Log.Debug("Copying directory {Source} to {Destination}", source, destination);
        // Get information about the source directory
        var directoryInfoSource = new DirectoryInfo(source);

        // Check if the source directory exists
        if (!directoryInfoSource.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {directoryInfoSource.FullName}");

        // Cache directories before we start copying
        var dirs = directoryInfoSource.GetDirectories();

        // Check if exists already
        var safePath = Directory.Exists(Path.Combine(destination, directoryInfoSource.Name))
            ? Path.Combine(destination, directoryInfoSource.Name + "_copy")
            : Path.Combine(destination, directoryInfoSource.Name);
        // Create the destination directory
        var directoryInfoDestination = Directory.CreateDirectory(safePath);

        // Get the files in the source directory and copy to the destination directory
        foreach (var file in directoryInfoSource.GetFiles())
        {
            var targetFilePath = Path.Combine(safePath, file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (!recursive) return directoryInfoDestination;
        foreach (var subDir in dirs)
        {
            var newDestinationDir = Path.Combine(safePath, subDir.Name);
            Copy(subDir.FullName, newDestinationDir, true);
        }

        return directoryInfoDestination;
    }

    public static void Extract(string path, string destination = "Extracted")
    {
        Log.Debug("Extracting archive {Path} to {Destination}", path, destination);
        using var archive = ArchiveFactory.Open(path);
        foreach (var entry in archive.Entries)
        {
            entry.WriteToDirectory(Path.Combine(path, destination), new ExtractionOptions()
            {
                ExtractFullPath = true,
                Overwrite = true
            });
        }
    }

    public static void ClearThumbnailsCache()
    {
        Log.Debug("Clearing thumbnails cache");
        if (!Directory.Exists(Settings.ThumbnailsPath)) return;
        var command = new SendMessageToStatusBarCommand(InfoBarSeverity.Success,
            "Thumbnails cache cleared successfully!");
        try
        {
            Directory.Delete(Settings.ThumbnailsPath, true);
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