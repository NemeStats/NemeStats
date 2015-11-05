using BoardGameGeekApiClient.Interfaces;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.BoardGameGeek
{
    public class BoardGameGeekGameDefinitionCreator : IBoardGameGeekGameDefinitionCreator
    {
        private IDataContext dataContext;
        private IBoardGameGeekApiClient boardGameGeekApiClient;

        public BoardGameGeekGameDefinitionCreator(IDataContext dataContext, IBoardGameGeekApiClient boardGameGeekApiClient)
        {
            this.dataContext = dataContext;
            this.boardGameGeekApiClient = boardGameGeekApiClient;
        }

        public int? CreateBoardGameGeekGameDefinition(int boardGameGeekGameDefinitionId, ApplicationUser currentUser)
        {
            var existingRecord = dataContext.FindById<BoardGameGeekGameDefinition>(boardGameGeekGameDefinitionId);

            if(existingRecord != null)
            {
                return boardGameGeekGameDefinitionId;
            }

            var gameDetails = boardGameGeekApiClient.GetGameDetails(boardGameGeekGameDefinitionId);

            if(gameDetails == null)
            {
                return null;
            }

            var newRecord = new BoardGameGeekGameDefinition
            {
                Id = boardGameGeekGameDefinitionId,
                Name = gameDetails.Name,
                Thumbnail = gameDetails.Thumbnail
            };

            dataContext.Save(newRecord, currentUser);

            return boardGameGeekGameDefinitionId;
        }
    }
}
