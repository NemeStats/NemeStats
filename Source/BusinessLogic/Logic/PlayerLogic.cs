using BusinessLogic.Models;
using System;
using System.Collections.Generic;
namespace BusinessLogic.Logic
{
    public interface PlayerLogic
    {
        Player GetPlayerDetails(int playerID);
        List<Player> GetAllPlayers(bool active);
    }
}
