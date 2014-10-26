using BusinessLogic.Models.Players;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Logic.Players
{
    public interface IPlayerSummaryBuilder
    {
        List<TopPlayer> GetTopPlayers(int numberOfPlayersToRetrieve);
    }
}
