using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Points;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BusinessLogic.DataAccess.Repositories
{
    public class EntityFrameworkPlayedGameRepository : PlayedGameRepository
    {
        internal const string EXCEPTION_MESSAGE_MUST_PASS_VALID_GAME_DEFINITION_ID = "Must pass a valid GameDefinitionId.";

        private DataContext dataContext;

        public EntityFrameworkPlayedGameRepository(DataContext applicationDataContext)
        {
            this.dataContext = applicationDataContext;
        }

        public PlayedGame GetPlayedGameDetails(int playedGameId, ApplicationUser currentUser)
        {         
            return dataContext.GetQueryable<PlayedGame>(currentUser)
                .Where(playedGame => playedGame.Id == playedGameId
                    && playedGame.GamingGroupId == currentUser.CurrentGamingGroupId)
                    .Include(playedGame => playedGame.GameDefinition)
                    .Include(playedGame => playedGame.PlayerGameResults
                        .Select(playerGameResult => playerGameResult.Player))
                    .FirstOrDefault();   
        }

        public List<PlayedGame> GetRecentGames(int numberOfGames, ApplicationUser currentUser)
        {
            List<PlayedGame> playedGames = dataContext.GetQueryable<PlayedGame>(currentUser)
                .Where(game => game.GamingGroupId == currentUser.CurrentGamingGroupId)
                .Include(playedGame => playedGame.GameDefinition)
                .Include(playedGame => playedGame.PlayerGameResults
                    .Select(playerGameResult => playerGameResult.Player))
                    .OrderByDescending(orderBy => orderBy.DatePlayed)
                .Take(numberOfGames)
                .ToList();

            //TODO this seems ridiculous but I can't see how to order a related entity in Entity Framework :(
            foreach(PlayedGame playedGame in playedGames)
            {
                playedGame.PlayerGameResults = playedGame.PlayerGameResults.OrderBy(orderBy => orderBy.GameRank).ToList();
            }

            return playedGames;
        }

    }
}
