using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.PlayedGames
{
    public interface PlayedGameRetriever
    {
        PlayedGame GetPlayedGameDetails(int playedGameId);
        List<PlayedGame> GetRecentGames(int numberOfGames, int gamingGroupId);
        List<PublicGameSummary> GetRecentPublicGames(int numberOfGames);
    }
}
