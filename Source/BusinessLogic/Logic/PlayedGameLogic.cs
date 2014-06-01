using System;
namespace BusinessLogic.Logic
{
    public interface PlayedGameLogic
    {
        BusinessLogic.Models.PlayedGame GetPlayedGameDetails(int playedGameId);
    }
}
