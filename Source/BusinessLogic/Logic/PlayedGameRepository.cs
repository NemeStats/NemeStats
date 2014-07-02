using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Points;
using BusinessLogic.Models.Identity;

namespace BusinessLogic.Models
{
    public class PlayedGameRepository : BusinessLogic.Models.PlayedGameLogic
    {
        internal const string EXCEPTION_MESSAGE_MUST_PASS_VALID_GAME_DEFINITION_ID = "Must pass a valid GameDefinitionId.";
        
        private NemeStatsDbContext dbContext;

        public PlayedGameRepository(NemeStatsDbContext context)
        {
            dbContext = context;
        }

        public PlayedGame GetPlayedGameDetails(int playedGameId)
        {         
            return dbContext.PlayedGames.Where(playedGame => playedGame.Id == playedGameId)
                .Include(playedGame => playedGame.GameDefinition)
                .Include(playedGame => playedGame.PlayerGameResults)
                .FirstOrDefault();   
        }

        public List<PlayedGame> GetRecentGames(int numberOfGames)
        {
            List<PlayedGame> playedGames = dbContext.PlayedGames.Include(playedGame => playedGame.GameDefinition)
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
        public PlayedGame CreatePlayedGame(NewlyCompletedGame newlyCompletedGame, UserContext user)
        {
            List<PlayerGameResult> playerGameResults = TransformNewlyCompletedGamePlayerRanksToPlayerGameResults(newlyCompletedGame);

            PlayedGame playedGame = TransformNewlyCompletedGameIntoPlayedGame(newlyCompletedGame, user, playerGameResults);

            dbContext.PlayedGames.Add(playedGame);
            dbContext.SaveChanges();

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
            UserContext user,
            List<PlayerGameResult> playerGameResults)
        {
            int numberOfPlayers = newlyCompletedGame.PlayerRanks.Count();
            PlayedGame playedGame = new PlayedGame()
            {
                GameDefinitionId = newlyCompletedGame.GameDefinitionId.Value,
                NumberOfPlayers = numberOfPlayers,
                PlayerGameResults = playerGameResults,
                DatePlayed = DateTime.UtcNow,
                GamingGroupId = user.GamingGroupId
            };
            return playedGame;
        }
    }
}
