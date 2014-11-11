using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using System.Linq;
using UI.Models.GameDefinitionModels;

namespace UI.Transformations
{
    public interface IGameDefinitionViewModelBuilder
    {
        GameDefinitionViewModel Build(GameDefinitionSummary gameDefinitionSummary, ApplicationUser currentUser);
    }
}
