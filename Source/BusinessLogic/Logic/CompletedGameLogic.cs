using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using System;
namespace BusinessLogic.Logic
{
    public interface CompletedGameLogic
    {
        PlayedGame CreatePlayedGame(NewlyCompletedGame newlyCompletedGame);
    }
}
