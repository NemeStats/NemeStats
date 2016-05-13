namespace BusinessLogic.Caching
{
    public interface ICacheRetriever
    {
        void AddItemToCache(string cacheKey, object data, int cacheExpirationInSeconds, string region = null);
        bool TryGetItemFromCache<T>(string cacheKey, out T itemFromCache) where T : class;
    }
}