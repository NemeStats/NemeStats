using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
namespace BusinessLogic.Models
{
    public interface PlayedGameLogic
    {
        PlayedGame GetPlayedGameDetails(int playedGameId);
        List<PlayedGame> GetRecentGames(int numberOfGames);
        PlayedGame CreatePlayedGame(NewlyCompletedGame newlyCompletedGame, UserContext user);
        List<PlayerGameResult> TransformNewlyCompletedGamePlayerRanksToPlayerGameResults(NewlyCompletedGame newlyCompletedGame);
        PlayedGame TransformNewlyCompletedGameIntoPlayedGame(
            NewlyCompletedGame newlyCompletedGame,
            UserContext user,
            List<PlayerGameResult> playerGameResults);
    }
}
