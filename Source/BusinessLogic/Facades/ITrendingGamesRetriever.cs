using System.Collections.Generic;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Facades
{
    public interface ITrendingGamesRetriever
    {
        List<TrendingGame> GetResults(TrendingGamesRequest trendingGamesRequest);
    }
}
