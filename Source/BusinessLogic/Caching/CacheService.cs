using System;
using System.Runtime.Caching;

namespace BusinessLogic.Caching
{
    public class CacheService : ICacheService
    {
        public void AddItemToCache<T>(string cacheKey, T item, int numberOfSecondsUntilExpiration)
        {
            var cacheItem = new CacheItem(cacheKey, item);

            var cachePolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = new DateTimeOffset(
                    DateTime.UtcNow.AddSeconds(numberOfSecondsUntilExpiration))
            };

            MemoryCache.Default.Add(cacheItem, cachePolicy);
        }

        public bool TryGetItemFromCache<TOutput>(string cacheKey, out TOutput itemInCache)
        {
            if (MemoryCache.Default.Contains(cacheKey))
            {
                itemInCache = (TOutput)MemoryCache.Default.Get(cacheKey);
                return true;
            }
            itemInCache = default(TOutput);
            return false;
        }

        public void EvictItemFromCache(string cacheKey)
        {
            MemoryCache.Default.Remove(cacheKey);
        }
    }
}
