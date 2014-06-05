using BusinessLogic.Models;
using System;
namespace BusinessLogic.Logic
{
    public interface CompletedGameLogic
    {
        PlayedGame CreatePlayedGame(BusinessLogic.Models.NewlyCompletedGame newlyCompletedGame);
    }
}
