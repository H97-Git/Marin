using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ImageMagick;
using Serilog;
using WaifuGallery.Models;

namespace WaifuGallery.Helpers;

public abstract class ThumbnailHelper
{
    public static bool Exists(FileInfo fileInfo, out string thumbnailPath)
    {
        var directoryName = fileInfo.Directory is null ? "Root" : fileInfo.Directory.Name;
        var currentPathInCache = Path.Combine(Settings.ThumbnailsPath, directoryName);
        Directory.CreateDirectory(currentPathInCache);
        thumbnailPath = PathHelper.GetAllImages(currentPathInCache)
            .FirstOrDefault(x => Path.GetFileName((string?) x) == fileInfo.Name) ?? currentPathInCache;
        return File.Exists(thumbnailPath);
    }

    public static async Task<Bitmap> GenerateAsync(FileInfo sourceFileInfo, FileInfo outputFileInfo)
    {
        Log.Debug("Generating thumb for: {Path}", sourceFileInfo.FullName);
        using var image = new MagickImage(sourceFileInfo);
        image.Resize(new MagickGeometry(200, 200)
        {
            IgnoreAspectRatio = false
        });
        await image.WriteAsync(outputFileInfo);
        return new Bitmap(outputFileInfo.FullName);
    }
}