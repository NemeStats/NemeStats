using BusinessLogic.Models.Players;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using BusinessLogic.Models.User;
namespace BusinessLogic.Models
{
    public interface PlayerLogic
    {
        PlayerDetails GetPlayerDetails(int playerID, int numberOfRecentGamesToRetrieve, UserContext requestingUserContext);
        List<Player> GetAllPlayers(bool active, UserContext requestingUserContext);
        PlayerStatistics GetPlayerStatistics(int playerId, UserContext requestingUserContext);
        Nemesis GetNemesis(int playerId, UserContext requestingUserContext);
    }
}
