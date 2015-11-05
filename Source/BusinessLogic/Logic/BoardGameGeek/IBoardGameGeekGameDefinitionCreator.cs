using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.BoardGameGeek
{
    public interface IBoardGameGeekGameDefinitionCreator
    {
        int? CreateBoardGameGeekGameDefinition(int boardGameGeekGameDefinitionId, ApplicationUser currentUser);
    }
}
