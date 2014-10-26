using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Logic.PlayedGames
{
    public interface IPlayedGameRetriever
    {
        PlayedGame GetPlayedGameDetails(int playedGameId);
        List<PlayedGame> GetRecentGames(int numberOfGames, int gamingGroupId);
        List<PublicGameSummary> GetRecentPublicGames(int numberOfGames);
    }
}
