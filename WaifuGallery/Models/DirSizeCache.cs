using System.Collections.Generic;

namespace WaifuGallery.Models;

public class DirSizeCache
{
    private static readonly Dictionary<string, long> _dirSizeCache = new();

    public static void AddOrUpdateDirSizeCache(string key, long value)
    {
        lock (_dirSizeCache)
        {
            _dirSizeCache[key] = value;
        }
    }

    public static bool TryGetValueDirSizeCache(string key, out long value)
    {
        lock (_dirSizeCache)
        {
            return _dirSizeCache.TryGetValue(key, out value);
        }
    }
}