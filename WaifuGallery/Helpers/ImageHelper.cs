using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Media.Imaging;
using WaifuGallery.ViewModels;

namespace WaifuGallery.Helpers;

public abstract class ImagesHelper
{
    #region Private Members

    /// <summary>
    /// Get all images in path and sort them in natural order: 1, 2, 3, 10, 11, 12
    /// </summary>
    /// <param name="di">The directory info</param>
    /// <returns>An array of FileInfo.FullName</returns>
    private static string[] GetAllImagesInPath(DirectoryInfo di)
    {
        return di.GetFiles()
            .Where(file => ImageFileExtensions.Contains(file.Extension.ToLower()))
            .OrderBy(f => f.Name, new NaturalSortComparer())
            .Select(file => file.FullName)
            .ToArray();
    }

    #endregion

    #region Public Members

    public static readonly string[] ImageFileExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp"];

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
    public static string[] GetAllImagesInPathFromString(string path)
    {
        var directoryInfo = new DirectoryInfo(path);
        return GetAllImagesInPath(directoryInfo);
    }

    /// <summary>
    /// Use a FileViewModel to get all images
    /// </summary>
    /// <param name="fileViewModel">FileViewModel with the path</param>
    /// <returns>An array of FileInfo.FullName</returns>
    public static string[] GetAllImagesInPathFromFileViewModel(FileViewModel fileViewModel)
    {
        var directoryInfo = fileViewModel.IsImage
            ? new DirectoryInfo(fileViewModel.ParentDirPath)
            : new DirectoryInfo(fileViewModel.FullPath);
        return GetAllImagesInPath(directoryInfo);
    }

    #endregion
}