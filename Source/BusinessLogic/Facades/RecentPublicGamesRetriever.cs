using System.Collections.Generic;
using BusinessLogic.Caching;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Facades
{
    public class RecentPublicGamesRetriever : Cacheable<int, List<PublicGameSummary>>
    {
        public const int CACHE_EXPIRATION_IN_SECONDS = 60 * 60;

        private readonly IPlayedGameRetriever _playedGameRetriever;

        public RecentPublicGamesRetriever(IPlayedGameRetriever playedGameRetriever)
        {
            _playedGameRetriever = playedGameRetriever;
        }

        internal override List<PublicGameSummary> GetFromSource(int numberOfGamesToRetrieve)
        {
            return _playedGameRetriever.GetRecentPublicGames(numberOfGamesToRetrieve);
        }

        internal override int GetCacheExpirationInSeconds()
        {
            return CACHE_EXPIRATION_IN_SECONDS;
        }
    }
}