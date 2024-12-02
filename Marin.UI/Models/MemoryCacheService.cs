using System;
using Microsoft.Extensions.Caching.Memory;

namespace Marin.UI.Models;

public static class MemoryCacheService
{
    private static readonly MemoryCacheOptions CacheOptions = new()
    {
        SizeLimit = 1024,
    };

    private static readonly MemoryCache Cache = new(CacheOptions);

    public static void Clear()
    {
        Cache.Clear();
    }

    public static void AddOrUpdate<T>(string key, T value, int expirationInMinutes = 30)
    {
        // Log.Debug("MemoryCacheService: AddOrUpdate, Adding {T} to cache", key);
        Cache.Set(key, value, new MemoryCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationInMinutes),
            Size = 1
        });
    }

    public static T? Get<T>(string key)
    {
        // Log.Debug("MemoryCacheService: Get, Getting {T} from cache", key);
        Cache.TryGetValue(key, out var value);
        if (value is not null)
            return (T) value;
        return default;
    }
}