using System.Collections.Generic;
using BusinessLogic.Caching;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Facades
{
    public class RecentPublicGamesRetriever : IRecentPublicGamesRetriever
    {
        public static readonly string CACHE_PREFIX = "GetRecevePublicGameSummaries";
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
            var cacheKey = GetCacheKey(numberOfRecentGamesToRetrieve);
            List<PublicGameSummary> returnValue;
            if (_cacheRetriever.TryGetItemFromCache(cacheKey, out returnValue))
            {
                return returnValue;
            }
            
            var data = _playedGameRetriever.GetRecentPublicGames(numberOfRecentGamesToRetrieve);
            _cacheRetriever.AddItemToCache(cacheKey, data, CACHE_EXPIRATION_IN_SECONDS);
            return data;
        }

        public static string GetCacheKey(int numberOfRecentGamesToRetrieve)
        {
            return string.Join("|", CACHE_PREFIX, numberOfRecentGamesToRetrieve);
        }
    }
}