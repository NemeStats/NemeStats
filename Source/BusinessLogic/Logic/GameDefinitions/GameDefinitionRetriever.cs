using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.GameDefinitions
{
    public interface GameDefinitionRetriever
    {
        IList<GameDefinition> GetAllGameDefinitions(ApplicationUser currentUser);
        GameDefinition GetGameDefinitionDetails(int id, int numberOfPlayedGamesToRetrieve);
    }
}
