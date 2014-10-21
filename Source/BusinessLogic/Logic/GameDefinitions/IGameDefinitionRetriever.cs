using BusinessLogic.Models;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Logic.GameDefinitions
{
    public interface IGameDefinitionRetriever
    {
        IList<GameDefinition> GetAllGameDefinitions(int gamingGroupId);
        GameDefinition GetGameDefinitionDetails(int id, int numberOfPlayedGamesToRetrieve);
    }
}
