using BusinessLogic.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.DataAccess.PlayerDataAccess
{
    public interface StatisticsDataAccess
    {
        Nemesis GetNemesis(int playerId);
    }
}
