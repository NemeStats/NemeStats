﻿using System;
using System.Runtime.Caching;
using BusinessLogic.Logic.Utilities;

namespace BusinessLogic.Caching
{
    public abstract class Cacheable<TInput, TOutput> : ICacheable<TInput, TOutput>
    {
        private readonly INemeStatsCacheManager _cacheManager;
        private readonly IDateUtilities _dateUtilities;

        protected Cacheable(IDateUtilities dateUtilities, INemeStatsCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            _dateUtilities = dateUtilities;
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

        public virtual int GetCacheExpirationInSeconds()
        {
            return _dateUtilities.GetNumberOfSecondsUntilEndOfDay();
        }

        public abstract TOutput GetFromSource(TInput inputParameter);

        public string GetCacheKey(TInput inputParameter)
        {
            return string.Join("|", GetType().GUID.ToString(), inputParameter.ToString());
        }
    }
}