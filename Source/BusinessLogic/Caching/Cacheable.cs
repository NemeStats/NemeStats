using System;
using System.Runtime.Caching;

namespace BusinessLogic.Caching
{
    public abstract class Cacheable<TInput, TOutput> : ICacheable<TInput, TOutput>
    {
        private readonly INemeStatsCacheManager _cacheManager;

        protected Cacheable(INemeStatsCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public TOutput GetResults(TInput inputParameter)
        {
            var cacheKey = GetCacheKey(inputParameter);
            TOutput itemFromCache;
            if (_cacheManager.TryGetItemFromCache<TOutput>(cacheKey, out itemFromCache))
            {
                return itemFromCache;
            }
            
            var itemFromSource = GetFromSource(inputParameter);
            
            _cacheManager.AddItemToCacheWithAbsoluteExpiration(cacheKey, itemFromSource, GetCacheExpirationInSeconds());
            return itemFromSource;
        }

        public abstract int GetCacheExpirationInSeconds();

        public abstract TOutput GetFromSource(TInput inputParameter);

        public string GetCacheKey(TInput inputParameter)
        {
            return string.Join("|", GetType().GUID.ToString(), inputParameter.ToString());
        }
    }
}
