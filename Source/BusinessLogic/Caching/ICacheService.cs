using System.Runtime.Caching;

namespace BusinessLogic.Caching
{
    public interface ICacheService
    {
        void Add<T>(string key,T item, CacheItemPolicy policy );
        T Get<T>(string key);
        void Delete(string key);
    }
}