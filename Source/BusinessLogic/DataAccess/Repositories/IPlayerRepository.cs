using System.Collections.Generic;
using BusinessLogic.Models.Nemeses;
using BusinessLogic.Models.Players;

namespace BusinessLogic.DataAccess.Repositories
{
    public interface IPlayerRepository
    {
        NemesisData GetNemesisData(int playerId);
        List<PlayerGameSummary> GetPlayerGameSummaries(int playerId);
    }
}
