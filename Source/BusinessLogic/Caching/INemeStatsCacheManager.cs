namespace BusinessLogic.Caching
{
    public interface INemeStatsCacheManager
    {
        void AddItemToCacheWithAbsoluteExpiration(string cacheKey, object item, int numberOfSecondsUntilExpiration);
        bool TryGetItemFromCache<TOutput>(string cacheKey, out TOutput itemInCache);
        void EvictItemFromCache(string cacheKey);
    }
}
