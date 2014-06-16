using BusinessLogic.Models.Players;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
namespace BusinessLogic.Models
{
    public interface PlayerLogic
    {
        PlayerDetails GetPlayerDetails(int playerID);
        List<Player> GetAllPlayers(bool active);
        PlayerStatistics GetPlayerStatistics(int playerId);
    }
}
