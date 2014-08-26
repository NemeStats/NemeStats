using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
namespace BusinessLogic.DataAccess.Repositories
{
    public interface PlayedGameRepository
    {
        PlayedGame GetPlayedGameDetails(int playedGameId);
        List<PlayedGame> GetRecentGames(int numberOfGames, ApplicationUser currentUser);
        List<PublicGameSummary> GetRecentPublicGames(int numberOfGames);
    }
}
