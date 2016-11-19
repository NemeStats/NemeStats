using System.Linq;
using BusinessLogic.Caching;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic.BoardGameGeekGameDefinitions
{
    public class CacheableGameDataRetriever : Cacheable<int, CacheableGameData>, ICacheableGameDataRetriever
    {
        private readonly IDataContext _dataContext;

        public CacheableGameDataRetriever(IDateUtilities dateUtilities, ICacheService cacheService, IDataContext dataContext) : base(dateUtilities, cacheService)
        {
            _dataContext = dataContext;
        }

        public override CacheableGameData GetFromSource(int boardGameGeekGameDefinitionId)
        {
            var result = _dataContext.GetQueryable<BoardGameGeekGameDefinition>().Where(x => x.Id == boardGameGeekGameDefinitionId)
                .Select(x => new CacheableGameData
                {
                    BoardGameGeekGameDefinitionId = x.Id,
                    BoardGameGeekAverageWeight = x.AverageWeight,
                    BoardGameGeekDescription = x.Description,
                    BoardGameGeekYearPublished = x.YearPublished,
                    ImageUrl = x.Image,
                    MaxPlayers = x.MaxPlayers,
                    MinPlayers = x.MinPlayers,
                    MinPlayTime = x.MinPlayTime,
                    MaxPlayTime = x.MaxPlayTime,
                    Name = x.Name,
                    ThumbnailImageUrl = x.Thumbnail,
                    BoardGameGeekMechanics = x.Mechanics.Select(y => y.MechanicName).ToList(),
                    BoardGameGeekCategories = x.Categories.Select(y => y.CategoryName).ToList(),
                    AveragePlayersPerGame = x.GameDefinitions.Average(y => y.PlayedGames.Select(p => p.NumberOfPlayers).Average()),
                    TotalNumberOfGamesPlayed = x.GameDefinitions.Sum(y => y.PlayedGames.Count),
                    TotalGamingGroupsWithThisGame = x.GameDefinitions.GroupBy(y => y.GamingGroupId).Select(z => z.Key).Count()
                }).FirstOrDefault();

            if (result == null)
            {
                throw new EntityDoesNotExistException(typeof(BoardGameGeekGameDefinition), boardGameGeekGameDefinitionId);
            }

            return result;
        }
    }
}