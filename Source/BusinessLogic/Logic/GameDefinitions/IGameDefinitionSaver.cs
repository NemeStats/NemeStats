using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System.Linq;

namespace BusinessLogic.Logic.GameDefinitions
{
    public interface IGameDefinitionSaver
    {
        GameDefinition Save(GameDefinition gameDefinition, ApplicationUser currentUser);
    }
}
