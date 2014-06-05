using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic
{
    public class CompletedGameRepository : BusinessLogic.Logic.CompletedGameLogic
    {
        internal const string EXCEPTION_MESSAGE_MUST_PASS_VALID_GAME_DEFINITION_ID = "Must pass a valid GameDefinitionId.";
        internal const string EXCEPTION_MESSAGE_MUST_PASS_AT_LEAST_ONE_PLAYER = "Must pass in at least one player";
        internal const string EXCEPTION_MESSAGE_GAME_MUST_HAVE_A_WINNER = "The game must have at least one winner (GameRank = 1)";
        internal const string EXCEPTION_MESSAGE_GAME_MUST_NOT_HAVE_A_GAP_IN_RANKS = "The game must not have gaps in the ranks. E.g. 1,1,2,3 is valid but 1,1,3,4 is not.";

        private NemeStatsDbContext dbContext = null;

        public CompletedGameRepository(NemeStatsDbContext context)
        {
            dbContext = context;
        }

        public PlayedGame CreatePlayedGame(NewlyCompletedGame newlyCompletedGame)
        {
            int numberOfPlayers = newlyCompletedGame.PlayerRanks.Count();

            Validate(newlyCompletedGame, numberOfPlayers);

            var playerGameResults = newlyCompletedGame.PlayerRanks.Select(x => new PlayerGameResult() { PlayerId = x.PlayerId, GameRank = x.GameRank }).ToList();

            PlayedGame playedGame = new PlayedGame()
            {
                GameDefinitionId = newlyCompletedGame.GameDefinitionId,
                NumberOfPlayers = numberOfPlayers,
                PlayerGameResults = playerGameResults,
                DatePlayed = DateTime.UtcNow
            };

            dbContext.PlayedGames.Add(playedGame);
            dbContext.SaveChanges();

            return playedGame;
        }

        private static void Validate(NewlyCompletedGame newlyCompletedGame, int numberOfPlayers)
        {
            if (newlyCompletedGame.GameDefinitionId == 0)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_MUST_PASS_VALID_GAME_DEFINITION_ID);
            }

            if (newlyCompletedGame.PlayerRanks.Count < 1)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_MUST_PASS_AT_LEAST_ONE_PLAYER);
            }

            if (newlyCompletedGame.PlayerRanks.FirstOrDefault(x => x.GameRank == 1) == null)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_GAME_MUST_HAVE_A_WINNER);
            }

            int numberOfPlayersCoveredSoFar = 0;

            //TODO review with Clean Code book club
            for (int i = 1; numberOfPlayersCoveredSoFar < numberOfPlayers; i++)
            {
                int numberOfPlayersWithThisRank = newlyCompletedGame.PlayerRanks.Count(x => x.GameRank == i);

                if (numberOfPlayersWithThisRank == 0)
                {
                    throw new ArgumentException(EXCEPTION_MESSAGE_GAME_MUST_NOT_HAVE_A_GAP_IN_RANKS);
                }

                numberOfPlayersCoveredSoFar += numberOfPlayersWithThisRank;
            }
        }
    }
}
