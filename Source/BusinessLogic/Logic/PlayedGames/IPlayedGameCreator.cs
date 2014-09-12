using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using System;
namespace BusinessLogic.Logic.PlayedGames
{
    public interface IPlayedGameCreator
    {
        PlayedGame CreatePlayedGame(NewlyCompletedGame newlyCompletedGame, ApplicationUser currentUser);
    }
}
