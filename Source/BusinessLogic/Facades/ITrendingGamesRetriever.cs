using System.Collections.Generic;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Facades
{
    public interface ITrendingGamesRetriever
    {
        IList<TrendingGame> GetResults(TrendingGamesRequest trendingGamesRequest);
    }
}
