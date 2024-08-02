using System.IO;

namespace WaifuGallery.Helpers;

public static class InMemoryCacheKeys
{
    public static string GetAllImagesInPathKey(string fullName, int depth) =>
        $"AllImagesInPath_{fullName.GetHashCode()}_{depth}";

    public static string GetDirectorySizeKey(FileSystemInfo fileSystemInfo) =>
        $"DirectorySize_{fileSystemInfo.FullName.GetHashCode()}";
}