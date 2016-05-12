namespace BusinessLogic.Caching
{
    public interface ICacheService
    {
        void AddItemToCache<T>(string cacheKey, T item, int numberOfSecondsUntilExpiration);
        bool TryGetItemFromCache<TOutput>(string cacheKey, out TOutput itemInCache);
        void EvictItemFromCache(string cacheKey);
    }
}
