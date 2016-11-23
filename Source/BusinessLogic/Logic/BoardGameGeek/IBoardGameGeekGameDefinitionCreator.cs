using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.BoardGameGeek
{
    public interface IBoardGameGeekGameDefinitionCreator
    {
        BoardGameGeekGameDefinition CreateBoardGameGeekGameDefinition(int boardGameGeekGameDefinitionId, ApplicationUser currentUser);
    }
}
