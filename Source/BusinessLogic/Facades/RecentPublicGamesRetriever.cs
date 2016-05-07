using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using BusinessLogic.Caching;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Facades
{
    public class RecentPublicGamesRetriever : IRecentPublicGamesRetriever
    {
        public string CachePrefix = "GetRecevePublicGameSummaries";
        public const int CACHE_EXPIRATION_IN_SECONDS = 60 * 60;

        private readonly IPlayedGameRetriever _playedGameRetriever;
        private readonly ICacheRetriever _cacheRetriever;

        public RecentPublicGamesRetriever(IPlayedGameRetriever playedGameRetriever, ICacheRetriever cacheRetriever)
        {
            _playedGameRetriever = playedGameRetriever;
            _cacheRetriever = cacheRetriever;
        }

        public List<PublicGameSummary> GetRecentPublicGames(int numberOfRecentGamesToRetrieve)
        {
            var cacheRetriever = new CacheRetriever();
            var cacheKey = GetCacheKey(numberOfRecentGamesToRetrieve);
            List<PublicGameSummary> returnValue;
            if (cacheRetriever.TryGetItemFromCache<List<PublicGameSummary>>(cacheKey, out returnValue))
            {
                return returnValue;
            }
            
            var data = _playedGameRetriever.GetRecentPublicGames(numberOfRecentGamesToRetrieve);
            cacheRetriever.AddItemToCache(cacheKey, data, CACHE_EXPIRATION_IN_SECONDS);
            return data;
        }

        private string GetCacheKey(int numberOfRecentGamesToRetrieve)
        {
            return string.Join("|", CachePrefix, numberOfRecentGamesToRetrieve);
        }
    }
}