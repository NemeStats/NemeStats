using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models.Nemeses;

namespace BusinessLogic.Logic.Nemeses
{
    public interface INemesisHistoryRetriever
    {
        NemesisHistoryData GetNemesisHistory(int playerId, int numberOfPreviousNemesisToReturn);
        List<NemesisChange> GetRecentNemesisChanges(int numberOfRecentNemeses);
    }
}
