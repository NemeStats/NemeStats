using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using System.Collections.Generic;
namespace BusinessLogic.Logic.Players
{
    public interface IPlayerRetriever
    {
        List<Player> GetAllPlayers(int gamingGroupId);
        List<Player> GetAllPlayersWithNemesisInfo(int gamingGroupId);
        PlayerDetails GetPlayerDetails(int playerId, int numberOfRecentGamesToRetrieve);
        PlayerStatistics GetPlayerStatistics(int playerId);
    }
}
