using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using UI.Models.GameDefinitionModels;

namespace UI.Transformations
{
    public interface IGameDefinitionSummaryViewModelBuilder
    {
        GameDefinitionSummaryViewModel Build(GameDefinitionSummary game, ApplicationUser currentUser);
    }
}
