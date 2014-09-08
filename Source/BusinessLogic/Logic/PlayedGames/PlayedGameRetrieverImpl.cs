using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BusinessLogic.Logic.PlayedGames
{
    public class PlayedGameRetrieverImpl : PlayedGameRetriever
    {
        private DataContext dataContext;

        public PlayedGameRetrieverImpl(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public List<PlayedGame> GetRecentGames(int numberOfGames, int gamingGroupId)
        {
            List<PlayedGame> playedGames = dataContext.GetQueryable<PlayedGame>()
                .Where(game => game.GamingGroupId == gamingGroupId)
                .Include(playedGame => playedGame.GameDefinition)
                .Include(playedGame => playedGame.PlayerGameResults
                    .Select(playerGameResult => playerGameResult.Player))
                    .OrderByDescending(orderBy => orderBy.DatePlayed)
                .Take(numberOfGames)
                .ToList();

            //TODO this seems ridiculous but I can't see how to order a related entity in Entity Framework :(
            foreach (PlayedGame playedGame in playedGames)
            {
                playedGame.PlayerGameResults = playedGame.PlayerGameResults.OrderBy(orderBy => orderBy.GameRank).ToList();
            }

            return playedGames;
        }


        public PlayedGame GetPlayedGameDetails(int playedGameId)
        {
            return dataContext.GetQueryable<PlayedGame>()
                .Where(playedGame => playedGame.Id == playedGameId)
                    .Include(playedGame => playedGame.GameDefinition)
                    .Include(playedGame => playedGame.PlayerGameResults
                        .Select(playerGameResult => playerGameResult.Player))
                    .FirstOrDefault();
        }

        public List<PublicGameSummary> GetRecentPublicGames(int numberOfGames)
        {
            return (from playedGame in dataContext.GetQueryable<PlayedGame>()
                    select new PublicGameSummary()
                    {
                        PlayedGameId = playedGame.Id,
                        GameDefinitionId = playedGame.GameDefinitionId,
                        GameDefinitionName = playedGame.GameDefinition.Name,
                        WinningPlayer = playedGame.PlayerGameResults.FirstOrDefault(player => player.GameRank == 1).Player,
                        DatePlayed = playedGame.DatePlayed
                    }).OrderByDescending(result => result.DatePlayed)
                                .Take(numberOfGames)
                                .ToList();
        }
    }
}
