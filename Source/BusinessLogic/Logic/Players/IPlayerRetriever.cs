using System;
using System.Collections.Generic;
namespace BusinessLogic.Logic.Players
{
    public interface IPlayerRetriever
    {
        List<BusinessLogic.Models.Player> GetAllPlayers(int gamingGroupId);
    }
}
