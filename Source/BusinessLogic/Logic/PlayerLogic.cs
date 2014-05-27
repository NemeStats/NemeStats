using System;
namespace BusinessLogic.Logic
{
    public interface PlayerLogic
    {
        BusinessLogic.Models.Player GetPlayerDetails(int playerID);
    }
}
