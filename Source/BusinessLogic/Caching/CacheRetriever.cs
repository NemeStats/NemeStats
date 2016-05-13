using System;
using System.Runtime.Caching;

namespace BusinessLogic.Caching
{
    public class CacheRetriever : ICacheRetriever
    {
        public bool TryGetItemFromCache<T>(string cacheKey, out T itemFromCache) where T : class
        {
            var memoryCache = MemoryCache.Default;
            if (memoryCache.Contains(cacheKey))
            {
                itemFromCache = memoryCache.Get(cacheKey) as T;
                return true;
            }

            itemFromCache = null;
            return false;
        }

        public void AddItemToCache(string cacheKey, object data, int cacheExpirationInSeconds, string region = null)
        {
            var cacheItem = new CacheItem(cacheKey, data);

            var cachePolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = new DateTimeOffset(
                    DateTime.UtcNow.AddSeconds(cacheExpirationInSeconds))
            };
            var memoryCache = MemoryCache.Default;
            memoryCache.Add(cacheItem, cachePolicy);
        }
    }
}
