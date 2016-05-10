using System;
using System.Runtime.Caching;

namespace BusinessLogic.Caching
{
    public abstract class Cacheable<TInput, TOutput> : ICacheable<TInput, TOutput>
    {
        public TOutput GetResults(TInput inputParameter)
        {
            var memoryCache = MemoryCache.Default;
            var cacheKey = GetCacheKey(inputParameter);
            if (memoryCache.Contains(cacheKey))
            {
                var itemFromCache = memoryCache.Get(cacheKey);

                return (TOutput)itemFromCache;
            }

            var itemFromDatabase = GetFromSource(inputParameter);
            var cacheItem = new CacheItem(cacheKey, itemFromDatabase);

            var cachePolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = new DateTimeOffset(
                    DateTime.UtcNow.AddSeconds(GetCacheExpirationInSeconds()))
            };
            memoryCache.Add(cacheItem, cachePolicy);
            return itemFromDatabase;
        }

        internal abstract int GetCacheExpirationInSeconds();

        internal abstract TOutput GetFromSource(TInput inputParameter);

        internal string GetCacheKey(TInput inputParameter)
        {
            return string.Join("|", GetType().GUID.ToString(), inputParameter.ToString());
        }
    }
}
