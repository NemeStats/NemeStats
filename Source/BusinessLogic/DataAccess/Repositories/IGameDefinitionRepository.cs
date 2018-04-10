using System.Collections.Generic;
using BusinessLogic.Models.Games;

namespace BusinessLogic.DataAccess.Repositories
{
    public interface IGameDefinitionRepository
    {
        IList<TrendingGame> GetTrendingGames(int maxNumberOfGames, int numberOfDaysOfTrendingGames);
    }
}
