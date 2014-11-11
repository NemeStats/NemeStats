using BusinessLogic.Models;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic.GameDefinitions
{
    public interface IGameDefinitionRetriever
    {
        IList<GameDefinitionSummary> GetAllGameDefinitions(int gamingGroupId);
        GameDefinitionSummary GetGameDefinitionDetails(int id, int numberOfPlayedGamesToRetrieve);
    }
}
