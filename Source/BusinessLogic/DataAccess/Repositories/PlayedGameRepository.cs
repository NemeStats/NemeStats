using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
namespace BusinessLogic.DataAccess.Repositories
{
    public interface PlayedGameRepository
    {
        PlayedGame GetPlayedGameDetails(int playedGameId, UserContext requestingUserContext);
        List<PlayedGame> GetRecentGames(int numberOfGames, UserContext requestingUserContext);
        PlayedGame CreatePlayedGame(NewlyCompletedGame newlyCompletedGame, UserContext requestingUserContext);
        List<PlayerGameResult> TransformNewlyCompletedGamePlayerRanksToPlayerGameResults(NewlyCompletedGame newlyCompletedGame);
        PlayedGame TransformNewlyCompletedGameIntoPlayedGame(
            NewlyCompletedGame newlyCompletedGame,
            int gamingGroupId,
            List<PlayerGameResult> playerGameResults);
    }
}
