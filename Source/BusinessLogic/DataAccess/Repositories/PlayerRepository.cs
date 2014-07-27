using BusinessLogic.Models.Players;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using BusinessLogic.Models.User;
namespace BusinessLogic.DataAccess.Repositories
{
    public interface PlayerRepository
    {
        PlayerDetails GetPlayerDetails(int playerID, int numberOfRecentGamesToRetrieve, ApplicationUser currentUser);
        List<Player> GetAllPlayers(bool active, ApplicationUser currentUser);
        PlayerStatistics GetPlayerStatistics(int playerId, ApplicationUser currentUser);
        Nemesis GetNemesis(int playerId, ApplicationUser currentUser);
        Player Save(Player player, ApplicationUser currentUser);
        void Delete(int playerId, ApplicationUser currentUser);
    }
}
