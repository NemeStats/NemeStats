using System;
using System.Linq;
using BusinessLogic.Caching;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using System.Collections;
using BusinessLogic.Facades;

namespace BusinessLogic.Logic.BoardGameGeekGameDefinitions
{
    public class UniversalStatsRetriever : Cacheable<int, UniversalGameStats>, IUniversalStatsRetriever
    {
        private readonly IDataContext _dataContext;

        public UniversalStatsRetriever(IDateUtilities dateUtilities, ICacheService cacheService, IDataContext dataContext) : base(dateUtilities, cacheService)
        {
            _dataContext = dataContext;
        }

        public override UniversalGameStats GetFromSource(int boardGameGeekGameDefinitionId)
        {
            var result = _dataContext.GetQueryable<BoardGameGeekGameDefinition>()
                .Where(x => x.Id == boardGameGeekGameDefinitionId)
                .Select(x => new UniversalGameStats
                {
                    AveragePlayersPerGame = x.GameDefinitions.Average(y => y.PlayedGames.Select(p => p.NumberOfPlayers).Average()),
                    TotalNumberOfGamesPlayed = x.GameDefinitions.Sum(y => y.PlayedGames.Count),
                    TotalGamingGroupsWithThisGame = x.GameDefinitions.Where(g=>g.Active && g.PlayedGames.Any()).GroupBy(y => y.GamingGroupId).Select(z => z.Key).Count()
                }).SingleOrDefault();

            if (result == null)
            {
                throw new EntityDoesNotExistException(typeof(BoardGameGeekGameDefinition), boardGameGeekGameDefinitionId);
            }

            return result;
        }
    }
}