using System.Collections.Generic;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Facades
{
    public interface IRecentPublicGamesRetriever
    {
        List<PublicGameSummary> GetResults(RecentlyPlayedGamesFilter filter);
    }
}