using BoardGameGeekApiClient.Interfaces;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

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

        public int? CreateBoardGameGeekGameDefinition(int boardGameGeekGameDefinitionId, ApplicationUser currentUser)
        {
            try
            {
                var existingRecord = _dataContext.FindById<BoardGameGeekGameDefinition>(boardGameGeekGameDefinitionId);
                if (existingRecord != null)
                {
                    return boardGameGeekGameDefinitionId;
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
                MaxPlayers = gameDetails.MaxPlayers,
                MinPlayers = gameDetails.MinPlayers,
                MaxPlayTime = gameDetails.MaxPlayTime,
                MinPlayTime= gameDetails.MinPlayTime,
                AverageWeight = gameDetails.AverageWeight,
                Description = gameDetails.Description,
                YearPublished = gameDetails.YearPublished,
                IsExpansion = gameDetails.IsExpansion,
                Rank = gameDetails.Rank
            };

            _dataContext.Save(newRecord, currentUser);

            // Save categories to BGG definition
            foreach (var x in gameDetails.Categories)
            {
                var gameToCategoryToSave = new BGGGameToCategory
                {
                    BoardGameGeekGameDefinitionId = boardGameGeekGameDefinitionId,
                    BoardGameGeekGameCategoryId = x.Id
                };

                _dataContext.Save(gameToCategoryToSave, currentUser);
            }



            return boardGameGeekGameDefinitionId;
        }
    }
}
