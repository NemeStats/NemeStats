using System.Collections.Generic;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.DataAccess.Repositories
{
    public interface IPlayedGameRepository
    {
        List<PublicGameSummary> GetRecentPublicGames(RecentlyPlayedGamesFilter filter);
    }
}