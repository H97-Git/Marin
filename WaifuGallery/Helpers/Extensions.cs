using System.Linq;

namespace WaifuGallery.Helpers;

public static class Extensions
{
    public static readonly string[] Archives = [".zip", ".rar", ".7z"];
    public static readonly string[] Images = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp"];
    public static readonly string[] Videos = [".mp4", ".mkv", ".avi", ".mov", ".wmv", ".webm", ".flv"];

    public static readonly string[] All =
        Archives.Concat(Images).Concat(Videos).ToArray();
}