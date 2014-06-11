using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Logic;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGameRepositoryTests
{
    public class CompletedGameLogicTests
    {
        private NemeStatsDbContext dbContext = null;
        PlayedGameLogic playedGameLogic = null;
        private int playedGameId = 2;

        [TestFixtureSetUp]
        public void SetUp()
        {
            dbContext = MockRepository.GenerateMock<NemeStatsDbContext>();
            playedGameLogic = new PlayedGameRepository(dbContext);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayedGameRepository.EXCEPTION_MESSAGE_MUST_PASS_AT_LEAST_TWO_PLAYERS)]
        public void ItRequiresMoreThanOnePlayer()
        {
            NewlyCompletedGame newlyCompletedGameThatHasNoPlayers = new NewlyCompletedGame();
            newlyCompletedGameThatHasNoPlayers.GameDefinitionId = playedGameId;
            newlyCompletedGameThatHasNoPlayers.PlayerRanks = new List<PlayerRank>() { new PlayerRank { GameRank = 1, PlayerId = 1 }};

            playedGameLogic.CreatePlayedGame(newlyCompletedGameThatHasNoPlayers);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayedGameRepository.EXCEPTION_MESSAGE_MUST_PASS_VALID_GAME_DEFINITION_ID)]
        public void ItRequiresAGame()
        {
            NewlyCompletedGame newlyCompletedGameThatHasNoGameDefinitionId = new NewlyCompletedGame();
            newlyCompletedGameThatHasNoGameDefinitionId.PlayerRanks = new List<PlayerRank>() { new PlayerRank() { PlayerId = 2, GameRank = 1 } };

            playedGameLogic.CreatePlayedGame(newlyCompletedGameThatHasNoGameDefinitionId);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayedGameRepository.EXCEPTION_MESSAGE_EACH_PLAYER_RANK_MUST_HAVE_A_PLAYER_ID)]
        public void ItRequiresEachPlayerRankToHaveAPlayer()
        {
            NewlyCompletedGame newlyCompletedGameThatHasNoPlayerId = new NewlyCompletedGame() { GameDefinitionId = 1 };
            newlyCompletedGameThatHasNoPlayerId.PlayerRanks = new List<PlayerRank>() { new PlayerRank() { GameRank = 1 } };

            playedGameLogic.CreatePlayedGame(newlyCompletedGameThatHasNoPlayerId);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayedGameRepository.EXCEPTION_MESSAGE_EACH_PLAYER_RANK_MUST_HAVE_A_GAME_RANK)]
        public void ItRequiresEachPlayerRankToAGameRank()
        {
            NewlyCompletedGame newlyCompletedGameThatHasNoGameRank = new NewlyCompletedGame() { GameDefinitionId = 1 };
            newlyCompletedGameThatHasNoGameRank.PlayerRanks = new List<PlayerRank>() 
                                            { 
                                                new PlayerRank() { PlayerId = 1, GameRank = 1 },
                                                new PlayerRank() { PlayerId = 2 } 
                                            };

            playedGameLogic.CreatePlayedGame(newlyCompletedGameThatHasNoGameRank);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayedGameRepository.EXCEPTION_MESSAGE_GAME_MUST_HAVE_A_WINNER)]
        public void ItRequiresAWinner()
        {
            NewlyCompletedGame newlyCompletedGameWithoutAWinner = new NewlyCompletedGame() { GameDefinitionId = 1 };
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 2 });
            playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 3 });
            newlyCompletedGameWithoutAWinner.PlayerRanks = playerRanks;

            //TODO possibly implement this way instead of ExpectedExceptions
            //Assert.Throws<ArgumentException>(()=> playedGameLogic.CreatePlayedGame(newlyCompletedGameWithoutAWinner))
            playedGameLogic.CreatePlayedGame(newlyCompletedGameWithoutAWinner);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = PlayedGameRepository.EXCEPTION_MESSAGE_GAME_MUST_NOT_HAVE_A_GAP_IN_RANKS)]
        public void ItRequiresContiguousRankingOfPlayers()
        {
            NewlyCompletedGame newlyCompletedGameWithUncontiguousGameRanks = new NewlyCompletedGame() { GameDefinitionId = 1 };
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 1 });
            playerRanks.Add(new PlayerRank() { PlayerId = 2, GameRank = 3 });
            newlyCompletedGameWithUncontiguousGameRanks.PlayerRanks = playerRanks;

            playedGameLogic.CreatePlayedGame(newlyCompletedGameWithUncontiguousGameRanks);
        }

        [Test]
        public void ItAcceptsAGameWithRanksOneTwoAndThreeRanks()
        {
            NewlyCompletedGame newlyCompletedGameWithUncontiguousGameRanks = new NewlyCompletedGame() { GameDefinitionId = 1 };
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 1 });
            playerRanks.Add(new PlayerRank() { PlayerId = 2, GameRank = 1 });
            playerRanks.Add(new PlayerRank() { PlayerId = 3, GameRank = 2 });
            playerRanks.Add(new PlayerRank() { PlayerId = 4, GameRank = 3 });
            newlyCompletedGameWithUncontiguousGameRanks.PlayerRanks = playerRanks;

            playedGameLogic.CreatePlayedGame(newlyCompletedGameWithUncontiguousGameRanks);
        }
    }
}
