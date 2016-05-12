using System.Collections.Generic;
using BusinessLogic.Caching;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Facades
{
    public class TrendingGamesRetriever : Cacheable<TrendingGamesRequest, List<TrendingGame>>, ITrendingGamesRetriever
    {
        private readonly IGameDefinitionRetriever _gameDefinitionRetriever;

        public TrendingGamesRetriever(
            IGameDefinitionRetriever gameDefinitionRetriever,
            IDateUtilities dateUtilities, 
            INemeStatsCacheManager cacheManager) : base(dateUtilities, cacheManager)
        {
            _gameDefinitionRetriever = gameDefinitionRetriever;
        }

        public override List<TrendingGame> GetFromSource(TrendingGamesRequest trendingGamesRequest)
        {
            return _gameDefinitionRetriever.GetTrendingGames(trendingGamesRequest.NumberOfTrendingGamesToShow, trendingGamesRequest.NumberOfDaysOfTrendingGames);
        }
    }
}