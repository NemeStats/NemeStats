using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using System;
namespace BusinessLogic.Logic.PlayedGames
{
    public interface PlayedGameCreator
    {
        PlayedGame CreatePlayedGame(NewlyCompletedGame newlyCompletedGame, ApplicationUser currentUser);
    }
}
