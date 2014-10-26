using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System.Linq;
using UI.Models.GameDefinitionModels;

namespace UI.Transformations
{
    public interface IGameDefinitionViewModelBuilder
    {
        GameDefinitionViewModel Build(GameDefinition gameDefinition, ApplicationUser currentUser);
    }
}
