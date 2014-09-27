using BusinessLogic.Models;
using System;
using System.Collections.Generic;
namespace BusinessLogic.Logic.Players
{
    public interface IPlayerRetriever
    {
        List<Player> GetAllPlayers(int gamingGroupId);
        List<Player> GetAllPlayersWithNemesisInfo(int gamingGroupId);
    }
}
