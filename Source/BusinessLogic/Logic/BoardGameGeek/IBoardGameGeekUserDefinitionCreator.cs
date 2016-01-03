using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.BoardGameGeek
{
    public interface IBoardGameGeekUserDefinitionCreator
    {
        int? CreateBoardGameGeekUserDefinition(string boardGameGeekUserName, ApplicationUser currentUser);
    }
}