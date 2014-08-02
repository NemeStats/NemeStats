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

        private NemeStatsDataContext applicationDataContext;

        public EntityFrameworkPlayedGameRepository(NemeStatsDataContext applicationDataContext)
        {
            this.applicationDataContext = applicationDataContext;
        }

        public PlayedGame GetPlayedGameDetails(int playedGameId, ApplicationUser currentUser)
        {         
            return applicationDataContext.GetQueryable<PlayedGame>(currentUser)
                .Where(playedGame => playedGame.Id == playedGameId
                    && playedGame.GamingGroupId == currentUser.CurrentGamingGroupId)
                    .Include(playedGame => playedGame.GameDefinition)
                    .Include(playedGame => playedGame.PlayerGameResults)
                    .FirstOrDefault();   
        }

        public List<PlayedGame> GetRecentGames(int numberOfGames, ApplicationUser currentUser)
        {
            List<PlayedGame> playedGames = applicationDataContext.GetQueryable<PlayedGame>(currentUser)
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

        //TODO need to have validation logic here (or on PlayedGame similar to what is on NewlyCompletedGame)
        public PlayedGame CreatePlayedGame(NewlyCompletedGame newlyCompletedGame, ApplicationUser currentUser)
        {
            List<PlayerGameResult> playerGameResults = TransformNewlyCompletedGamePlayerRanksToPlayerGameResults(newlyCompletedGame);

            PlayedGame playedGame = TransformNewlyCompletedGameIntoPlayedGame(
                newlyCompletedGame, 
                //TODO should throw some kind of exception if GamingGroupId is null
                currentUser.CurrentGamingGroupId.Value, 
                playerGameResults);

            applicationDataContext.Save(playedGame, currentUser);

            return playedGame;
        }

        //TODO this should be out of the repository
        public virtual List<PlayerGameResult> TransformNewlyCompletedGamePlayerRanksToPlayerGameResults(NewlyCompletedGame newlyCompletedGame)
        {
            int numberOfPlayers = newlyCompletedGame.PlayerRanks.Count();
            var playerGameResults = newlyCompletedGame.PlayerRanks
                                        .Select(playerRank => new PlayerGameResult()
                                        {
                                            PlayerId = playerRank.PlayerId.Value,
                                            GameRank = playerRank.GameRank.Value,
                                            //TODO maybe too functional in style? Is there a better way?
                                            GordonPoints = GordonPoints.CalculateGordonPoints(numberOfPlayers, playerRank.GameRank.Value)
                                        })
                                        .ToList();
            return playerGameResults;
        }

        //TODO this should be in its own class
        public PlayedGame TransformNewlyCompletedGameIntoPlayedGame(
            NewlyCompletedGame newlyCompletedGame,
            int gamingGroupId,
            List<PlayerGameResult> playerGameResults)
        {
            int numberOfPlayers = newlyCompletedGame.PlayerRanks.Count();
            PlayedGame playedGame = new PlayedGame()
            {
                GameDefinitionId = newlyCompletedGame.GameDefinitionId.Value,
                NumberOfPlayers = numberOfPlayers,
                PlayerGameResults = playerGameResults,
                DatePlayed = DateTime.UtcNow,
                GamingGroupId = gamingGroupId
            };
            return playedGame;
        }
    }
}
