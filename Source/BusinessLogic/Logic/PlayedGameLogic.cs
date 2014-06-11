using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using System;
using System.Collections.Generic;
namespace BusinessLogic.Logic
{
    public interface PlayedGameLogic
    {
        PlayedGame GetPlayedGameDetails(int playedGameId);
        List<PlayedGame> GetRecentGames(int numberOfGames);
        PlayedGame CreatePlayedGame(NewlyCompletedGame newlyCompletedGame);
    }
}
