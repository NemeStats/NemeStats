using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic
{
    public class PlayedGameRepository : BusinessLogic.Logic.PlayedGameLogic
    {
        internal const string EXCEPTION_MESSAGE_MUST_PASS_VALID_GAME_DEFINITION_ID = "Must pass a valid GameDefinitionId.";
        
        private NemeStatsDbContext dbContext;

        public PlayedGameRepository(NemeStatsDbContext context)
        {
            dbContext = context;
        }

        public PlayedGame GetPlayedGameDetails(int playedGameId)
        {
            return dbContext.PlayedGames.Where(playedGame => playedGame.Id == playedGameId).FirstOrDefault();                
        }

        public List<PlayedGame> GetRecentGames(int numberOfGames)
        {
            return dbContext.PlayedGames.Include(p => p.GameDefinition)
                .Take(numberOfGames)
                .OrderByDescending(x => x.DatePlayed)
                .ToList();
        }

        public PlayedGame CreatePlayedGame(NewlyCompletedGame newlyCompletedGame)
        {
            if(!newlyCompletedGame.GameDefinitionId.HasValue)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_MUST_PASS_VALID_GAME_DEFINITION_ID);
            }
            
            var playerGameResults = newlyCompletedGame.PlayerRanks
                                        .Select(playerRank => new PlayerGameResult()
                                        {
                                            PlayerId = playerRank.PlayerId.Value,
                                            GameRank = playerRank.GameRank.Value
                                        })
                                        .ToList();

            int numberOfPlayers = newlyCompletedGame.PlayerRanks.Count();
            PlayedGame playedGame = new PlayedGame()
            {
                GameDefinitionId = newlyCompletedGame.GameDefinitionId.Value,
                NumberOfPlayers = numberOfPlayers,
                PlayerGameResults = playerGameResults,
                DatePlayed = DateTime.UtcNow
            };

            dbContext.PlayedGames.Add(playedGame);
            dbContext.SaveChanges();

            return playedGame;
        }
    }
}
