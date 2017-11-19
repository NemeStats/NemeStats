using BoardGameGeekApiClient.Interfaces;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System.Linq;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic.BoardGameGeek
{
    public class BoardGameGeekGameDefinitionCreator : IBoardGameGeekGameDefinitionCreator
    {
        private readonly IDataContext _dataContext;
        private readonly IBoardGameGeekApiClient _boardGameGeekApiClient;

        public BoardGameGeekGameDefinitionCreator(IDataContext dataContext, IBoardGameGeekApiClient boardGameGeekApiClient)
        {
            _dataContext = dataContext;
            _boardGameGeekApiClient = boardGameGeekApiClient;
        }

        public BoardGameGeekGameDefinition CreateBoardGameGeekGameDefinition(int boardGameGeekGameDefinitionId)
        {
            try
            {
                var existingRecord = _dataContext.FindById<BoardGameGeekGameDefinition>(boardGameGeekGameDefinitionId);
                if (existingRecord != null)
                {
                    return existingRecord;
                }
            }
            catch (EntityDoesNotExistException)
            {
                //this is OK, just means we need to create a new one
            }

            var gameDetails = _boardGameGeekApiClient.GetGameDetails(boardGameGeekGameDefinitionId);

            if (gameDetails == null)
            {
                return null;
            }

            var newRecord = new BoardGameGeekGameDefinition
            {
                Id = boardGameGeekGameDefinitionId,
                Name = gameDetails.Name,
                Thumbnail = gameDetails.Thumbnail,
                Image = gameDetails.Image,
                MaxPlayers = gameDetails.MaxPlayers,
                MinPlayers = gameDetails.MinPlayers,
                MaxPlayTime = gameDetails.MaxPlayTime,
                MinPlayTime = gameDetails.MinPlayTime,
                AverageWeight = gameDetails.AverageWeight,
                Description = gameDetails.Description,
                YearPublished = gameDetails.YearPublished,
                IsExpansion = gameDetails.IsExpansion,
                Rank = gameDetails.Rank
            };

            _dataContext.AdminSave(newRecord);

            // Save categories to BGG definition
            foreach (var category in gameDetails.Categories)
            {
                var existentCategory = _dataContext.GetQueryable<BoardGameGeekGameCategory>().FirstOrDefault(c => c.BoardGameGeekGameCategoryId == category.Id);

                if (existentCategory == null)
                {
                    existentCategory = new BoardGameGeekGameCategory()
                    {
                        BoardGameGeekGameCategoryId = category.Id,
                        CategoryName = category.Category
                    };
                }

                newRecord.Categories.Add(existentCategory);
            }

            foreach (var mechanic in gameDetails.Mechanics)
            {
                var existentMechanic = _dataContext.GetQueryable<BoardGameGeekGameMechanic>().FirstOrDefault(c => c.BoardGameGeekGameMechanicId == mechanic.Id);

                if (existentMechanic == null)
                {
                    existentMechanic = new BoardGameGeekGameMechanic()
                    {
                        BoardGameGeekGameMechanicId = mechanic.Id,
                        MechanicName = mechanic.Mechanic
                    };
                }

                newRecord.Mechanics.Add(existentMechanic);
            }

            return newRecord;
        }
    }
}
