using Microsoft.Extensions.Caching.Memory;

namespace Project.Shared;


    public static class MemoryCacheExtensions
    {
        public static IEnumerable<T> GetKeys<T>(this IMemoryCache memoryCache)
        {
            var cacheEntriesField = typeof(MemoryCache).GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var cacheEntriesCollection = cacheEntriesField.GetValue(memoryCache) as dynamic;
            foreach (var cacheEntry in cacheEntriesCollection)
            {
                var cacheItem = cacheEntry.GetType().GetProperty("Key").GetValue(cacheEntry, null);
                if (cacheItem is T key)
                {
                    yield return key;
                }
            }
        }
    }


