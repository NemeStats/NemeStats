using BusinessLogic.Models;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic.GameDefinitions
{
    public interface IGameDefinitionRetriever
    {
        IList<GameDefinitionSummary> GetAllGameDefinitions(int gamingGroupId);
        GameDefinition GetGameDefinitionDetails(int id, int numberOfPlayedGamesToRetrieve);
    }
}
