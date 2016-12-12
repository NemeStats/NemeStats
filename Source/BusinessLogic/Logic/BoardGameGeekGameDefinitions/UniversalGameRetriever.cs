using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Facades;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.BoardGameGeekGameDefinitions
{
    public class UniversalGameRetriever : IUniversalGameRetriever
    {
        public const int DEFAULT_NUMBER_OF_GAMES = 5;

        private readonly IBoardGameGeekGameDefinitionInfoRetriever _boardGameGeekGameDefinitionInfoRetriever;
        private readonly IGameDefinitionRetriever _gameDefinitionRetriever;
        private readonly IDataContext _dataContext;
        private readonly IUniversalStatsRetriever _universalStatsRetriever;
        private readonly IRecentPublicGamesRetriever _recentPublicGamesRetriever;
        private readonly IUniversalTopChampionsRetreiver _universalTopChampionsRetreiver;

        public UniversalGameRetriever(
            IBoardGameGeekGameDefinitionInfoRetriever boardGameGeekGameDefinitionInfoRetriever, 
            IGameDefinitionRetriever gameDefinitionRetriever, 
            IDataContext dataContext, 
            IUniversalStatsRetriever universalStatsRetriever, 
            IRecentPublicGamesRetriever recentPublicGamesRetriever,
            IUniversalTopChampionsRetreiver universalTopChampionsRetreiver)
        {
            _boardGameGeekGameDefinitionInfoRetriever = boardGameGeekGameDefinitionInfoRetriever;
            _gameDefinitionRetriever = gameDefinitionRetriever;
            _dataContext = dataContext;
            _universalStatsRetriever = universalStatsRetriever;
            _recentPublicGamesRetriever = recentPublicGamesRetriever;
            _universalTopChampionsRetreiver = universalTopChampionsRetreiver;
        }

        public BoardGameGeekGameSummary GetBoardGameGeekGameSummary(int boardGameGeekGameDefinitionId, ApplicationUser currentUser, int numberOfRecentlyPlayedGamesToRetrieve = DEFAULT_NUMBER_OF_GAMES)
        {
            var topChampions = _universalTopChampionsRetreiver.GetFromSource(boardGameGeekGameDefinitionId);
            var boardGameGeekInfo = _boardGameGeekGameDefinitionInfoRetriever.GetResults(boardGameGeekGameDefinitionId);
            var universalStats = _universalStatsRetriever.GetResults(boardGameGeekGameDefinitionId);
            var gamingGroupGameDefinitionSummary = GetGamingGroupGameDefinitionSummary(boardGameGeekGameDefinitionId, currentUser.CurrentGamingGroupId, numberOfRecentlyPlayedGamesToRetrieve);

            var filter = new RecentlyPlayedGamesFilter
            {
                BoardGameGeekGameDefinitionId = boardGameGeekGameDefinitionId,
                NumberOfGamesToRetrieve = numberOfRecentlyPlayedGamesToRetrieve
            };
            var recentlyPlayedPublicGames = _recentPublicGamesRetriever.GetResults(filter);

            return new BoardGameGeekGameSummary
            {
                BoardGameGeekInfo = boardGameGeekInfo,
                UniversalGameStats = universalStats,
                GamingGroupGameDefinitionSummary = gamingGroupGameDefinitionSummary,
                RecentlyPlayedGames = recentlyPlayedPublicGames,
                TopChampions = topChampions
            };
        }

        public List<UniversalGameSitemapInfo> GetAllActiveBoardGameGeekGameDefinitionSitemapInfos()
        {
            throw new System.NotImplementedException();
        }

        private GameDefinitionSummary GetGamingGroupGameDefinitionSummary(int boardGameGeekGameDefinitionId, int currentUserCurrentGamingGroupId, int numberOfRecentlyPlayedGamesToShow)
        {
            GameDefinitionSummary summary = null;

            var gameDefinitionId = _dataContext.GetQueryable<GameDefinition>().Where(
                    x => x.BoardGameGeekGameDefinitionId == boardGameGeekGameDefinitionId
                         && x.GamingGroupId == currentUserCurrentGamingGroupId)
                .Select(x => x.Id)
                .FirstOrDefault();

            if (gameDefinitionId != default(int))
            {
                summary = _gameDefinitionRetriever.GetGameDefinitionDetails(gameDefinitionId, numberOfRecentlyPlayedGamesToShow);
            }

            return summary;
        }


    }
}