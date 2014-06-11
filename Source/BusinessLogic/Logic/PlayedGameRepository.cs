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
        internal const string EXCEPTION_MESSAGE_EACH_PLAYER_RANK_MUST_HAVE_A_PLAYER_ID = "Each PlayerRank must have a valid PlayerId.";
        internal const string EXCEPTION_MESSAGE_EACH_PLAYER_RANK_MUST_HAVE_A_GAME_RANK = "Each PlayerRank must have a valid GameRank.";
        internal const string EXCEPTION_MESSAGE_MUST_PASS_AT_LEAST_TWO_PLAYERS = "Must pass in at least two players.";
        internal const string EXCEPTION_MESSAGE_GAME_MUST_HAVE_A_WINNER = "The game must have at least one winner (GameRank = 1).";
        internal const string EXCEPTION_MESSAGE_GAME_MUST_NOT_HAVE_A_GAP_IN_RANKS = "The game must not have gaps in the ranks. E.g. 1,1,2,3 is valid but 1,1,3,4 is not.";

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
            //TODO MOVE TO SEPARATE VALIDATOR OBJECT. Talk to Ryan.
            Validate(newlyCompletedGame);

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

        private static void Validate(NewlyCompletedGame newlyCompletedGame)
        {
            ValidateGameDefinition(newlyCompletedGame);
            ValidatePlayerRanks(newlyCompletedGame.PlayerRanks);
        }

        private static void ValidateGameDefinition(NewlyCompletedGame newlyCompletedGame)
        {
            if (!newlyCompletedGame.GameDefinitionId.HasValue)
            {
                //TODO possibly include extra data to be helpful (e.g. the game definition that caused the problem)
                throw new ArgumentException(EXCEPTION_MESSAGE_MUST_PASS_VALID_GAME_DEFINITION_ID);
            }
        }

        private static void ValidatePlayerRanks(List<PlayerRank> playerRanks)
        {
            ValidateThatAllPlayerIdsAreSet(playerRanks);
            ValidateThatAllGameRanksAreSet(playerRanks);
            ValidateThatThereAreAtLeastTwoPlayers(playerRanks);
            ValidateThatThereIsAWinner(playerRanks);
            ValidateContiguousGameRanks(playerRanks);
        }

        private static void ValidateThatAllPlayerIdsAreSet(List<PlayerRank> playerRanks)
        {
            if (playerRanks.Any(playerRank => playerRank.PlayerId == null))
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_EACH_PLAYER_RANK_MUST_HAVE_A_PLAYER_ID);
            }
        }

        private static void ValidateThatAllGameRanksAreSet(List<PlayerRank> playerRanks)
        {
            if (playerRanks.Any(playerRank => playerRank.GameRank == null))
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_EACH_PLAYER_RANK_MUST_HAVE_A_GAME_RANK);
            }
        }

        private static void ValidateThatThereAreAtLeastTwoPlayers(List<PlayerRank> playerRanks)
        {
            if (playerRanks.Count < 2)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_MUST_PASS_AT_LEAST_TWO_PLAYERS);
            }
        }

        private static void ValidateThatThereIsAWinner(List<PlayerRank> playerRanks)
        {
            if (!playerRanks.Any(playerRank => playerRank.GameRank == 1))
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_GAME_MUST_HAVE_A_WINNER);
            }
        }

        private static void ValidateContiguousGameRanks(List<PlayerRank> playerRanks)
        {
            int numberOfPlayers = playerRanks.Count();
            int numberOfPlayersCoveredSoFar = 0;
            //TODO review with Clean Code book club
            for (int i = 1; numberOfPlayersCoveredSoFar < numberOfPlayers; i++)
            {
                int numberOfPlayersWithThisRank = playerRanks.Count(x => x.GameRank == i);

                if (numberOfPlayersWithThisRank == 0)
                {
                    throw new ArgumentException(EXCEPTION_MESSAGE_GAME_MUST_NOT_HAVE_A_GAP_IN_RANKS);
                }

                numberOfPlayersCoveredSoFar += numberOfPlayersWithThisRank;
            }
        }
    }
}
