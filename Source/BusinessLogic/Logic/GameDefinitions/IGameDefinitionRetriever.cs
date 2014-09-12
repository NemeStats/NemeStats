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
    public interface IGameDefinitionRetriever
    {
        IList<GameDefinition> GetAllGameDefinitions(int gamingGroupId);
        GameDefinition GetGameDefinitionDetails(int id, int numberOfPlayedGamesToRetrieve);
    }
}
