using BusinessLogic.Logic.Utilities;

namespace BusinessLogic.Caching
{
    public abstract class Cacheable<TInput, TOutput> : ICacheable<TInput, TOutput>
    {
        private readonly ICacheService _cacheService;
        private readonly IDateUtilities _dateUtilities;

        protected Cacheable(IDateUtilities dateUtilities, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _dateUtilities = dateUtilities;
        }

        public TOutput GetResults(TInput inputParameter)
        {
            var cacheKey = GetCacheKey(inputParameter);
            TOutput itemFromCache;
            if (_cacheService.TryGetItemFromCache(cacheKey, out itemFromCache))
            {
                return itemFromCache;
            }
            
            var itemFromSource = GetFromSource(inputParameter);
            
            _cacheService.AddItemToCache(cacheKey, itemFromSource, GetCacheExpirationInSeconds());
            return itemFromSource;
        }

        public void EvictFromCache(TInput inputParameter)
        {
            _cacheService.EvictItemFromCache(GetCacheKey(inputParameter));
        }

        public virtual int GetCacheExpirationInSeconds()
        {
            return _dateUtilities.GetNumberOfSecondsUntilEndOfDay();
        }

        public abstract TOutput GetFromSource(TInput inputParameter);

        public virtual string GetCacheKey(TInput inputParameter)
        {
            return string.Join("|", GetType().GUID.ToString(), inputParameter.GetHashCode());
        }
    }
}
