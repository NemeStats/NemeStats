using BusinessLogic.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.Players
{
    public interface IPlayerSummaryBuilder
    {
        List<TopPlayer> GetTopPlayers(int numberOfPlayersToRetrieve);
    }
}
