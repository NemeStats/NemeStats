using System.Runtime.Caching;

namespace BusinessLogic.Caching
{
    public class MemoryCacheService : ICacheService
    {
        public void Add<T>(string key, T item, CacheItemPolicy policy)
        {
            var cacheItem = new CacheItem(key, item);
            MemoryCache.Default.Add(cacheItem, policy);
        }

        public T Get<T>(string key)
        {
            return (T) MemoryCache.Default.Get(key);
        }

        public void Delete(string key)
        {
            MemoryCache.Default.Remove(key);
        }
    }
}