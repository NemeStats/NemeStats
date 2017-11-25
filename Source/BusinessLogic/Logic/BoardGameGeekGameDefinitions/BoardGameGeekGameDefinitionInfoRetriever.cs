using System.Linq;
using BusinessLogic.Caching;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic.BoardGameGeekGameDefinitions
{
    public class BoardGameGeekGameDefinitionInfoRetriever : Cacheable<int, BoardGameGeekInfo>, IBoardGameGeekGameDefinitionInfoRetriever
    {
        private readonly IDataContext _dataContext;

        public BoardGameGeekGameDefinitionInfoRetriever(IDateUtilities dateUtilities, ICacheService cacheService, IDataContext dataContext) : base(dateUtilities, cacheService)
        {
            _dataContext = dataContext;
        }

        public override BoardGameGeekInfo GetFromSource(int boardGameGeekGameDefinitionId)
        {
            var result = _dataContext.GetQueryable<BoardGameGeekGameDefinition>().Where(x => x.Id == boardGameGeekGameDefinitionId)
                .Select(x =>  new BoardGameGeekInfo
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
                        GameName = x.Name,
                        ThumbnailImageUrl = x.Thumbnail,
                        BoardGameGeekMechanics = x.Mechanics.Select(y => y.MechanicName).ToList(),
                        BoardGameGeekCategories = x.Categories.Select(y => y.CategoryName).ToList()
                }).FirstOrDefault();

            if (result == null)
            {
                throw new EntityDoesNotExistException<BoardGameGeekGameDefinition>(boardGameGeekGameDefinitionId);
            }

            return result;
        }
    }
}