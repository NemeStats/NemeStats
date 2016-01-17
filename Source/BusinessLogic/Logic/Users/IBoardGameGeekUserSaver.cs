using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Users
{
    public interface IBoardGameGeekUserSaver
    {
        BoardGameGeekUserDefinition CreateUserDefintion(CreateBoardGameGeekUserDefinitionRequest request,
            ApplicationUser currentUser);
    }
}