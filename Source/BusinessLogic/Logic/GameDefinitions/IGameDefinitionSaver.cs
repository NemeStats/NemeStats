using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Logic.GameDefinitions
{
    public interface IGameDefinitionSaver
    {
        GameDefinition Save(GameDefinition gameDefinition, ApplicationUser currentUser);
    }
}
