using System;
using System.Runtime.Caching;

namespace BusinessLogic.Caching
{
    public class NemeStatsCacheManager : INemeStatsCacheManager
    {
        private readonly ICacheService _cacheService;

        public NemeStatsCacheManager(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public void AddItemToCacheWithAbsoluteExpiration(string cacheKey, object item, int numberOfSecondsUntilExpiration)
        {
            var cachePolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = new DateTimeOffset(
                    DateTime.UtcNow.AddSeconds(numberOfSecondsUntilExpiration))
            };

            _cacheService.Add(cacheKey,item, cachePolicy);
        }

        public bool TryGetItemFromCache<TOutput>(string cacheKey, out TOutput itemInCache)
        {
            if (MemoryCache.Default.Contains(cacheKey))
            {
                itemInCache = _cacheService.Get<TOutput>(cacheKey);
                return true;
            }
            itemInCache = default(TOutput);
            return false;
        }

        public void EvictItemFromCache(string cacheKey)
        {
            _cacheService.Delete(cacheKey);
        }
    }
}
