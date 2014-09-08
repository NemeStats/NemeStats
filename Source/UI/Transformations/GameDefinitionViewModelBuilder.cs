using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UI.Models.GameDefinitionModels;

namespace UI.Transformations
{
    public interface GameDefinitionViewModelBuilder
    {
        GameDefinitionViewModel Build(GameDefinition gameDefinition, ApplicationUser currentUser);
    }
}
