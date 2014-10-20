using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLogic.Models;
using BusinessLogic.Models.Nemeses;

namespace BusinessLogic.Logic.Nemeses
{
    public interface INemesisHistoryRetriever
    {
        NemesisHistoryData GetNemesisHistory(int playerId, int numberOfPreviousNemesisToReturn);
    }
}
