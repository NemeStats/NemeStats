using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.GameDefinitions
{
    public interface ICreateGameDefinitionComponent
    {
        GameDefinition Execute(CreateGameDefinitionRequest createGameDefinitionRequest, ApplicationUser currentUser);

        GameDefinition ExecuteTransaction(CreateGameDefinitionRequest createGameDefinitionRequest, ApplicationUser currentUser, IDataContext dataContext);
    }
}