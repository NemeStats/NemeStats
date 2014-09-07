using BusinessLogic.Models.Players;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using BusinessLogic.Models.User;
namespace BusinessLogic.DataAccess.Repositories
{
    public interface PlayerRepository
    {
        PlayerDetails GetPlayerDetails(int playerID, int numberOfRecentGamesToRetrieve);
        PlayerStatistics GetPlayerStatistics(int playerId);
        Nemesis GetNemesis(int playerId);
    }
}
