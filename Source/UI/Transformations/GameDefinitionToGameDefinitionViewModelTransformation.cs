using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UI.Models.GameDefinitionModels;

namespace UI.Transformations
{
    public interface GameDefinitionToGameDefinitionViewModelTransformation
    {
        GameDefinitionViewModel Build(GameDefinition gameDefinition);
    }
}
