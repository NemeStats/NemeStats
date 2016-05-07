using System.Collections.Generic;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Facades
{
    public interface IRecentPublicGamesRetriever
    {
        List<PublicGameSummary> GetRecentPublicGames(int numberOfRecentGamesToRetrieve);
    }
}
