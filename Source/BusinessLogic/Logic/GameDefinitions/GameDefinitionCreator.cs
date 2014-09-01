using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Logic.GameDefinitions
{
    public interface GameDefinitionCreator
    {
        GameDefinition CreateGameDefinition(string gameDefinitionName, ApplicationUser currentUser);
    }
}
