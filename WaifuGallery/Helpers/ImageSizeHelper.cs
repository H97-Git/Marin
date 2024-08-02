using Avalonia;
using Avalonia.Media.Imaging;
using Serilog;

namespace WaifuGallery.Helpers;

public static class ImageSizeHelper
{
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
        Log.Debug("GetScaledSize: {DesiredSize}", desiredSize);
        var isPortrait = image.Size.Width < image.Size.Height;
        return isPortrait
            ? GetScaledSizeByHeight(image, desiredSize)
            : GetScaledSizeByWidth(image, desiredSize);
    }
}