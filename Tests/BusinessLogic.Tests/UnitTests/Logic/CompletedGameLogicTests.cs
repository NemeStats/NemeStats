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

namespace BusinessLogic.Tests.UnitTests.Logic
{
    public class CompletedGameLogicTests
    {
        private NemeStatsDbContext dbContext = null;
        CompletedGameRepository playedGameLogic = null;
        private int playedGameId = 2;

        [TestFixtureSetUp]
        public void SetUp()
        {
            dbContext = MockRepository.GenerateMock<NemeStatsDbContext>();
            playedGameLogic = new CompletedGameRepository(dbContext);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = CompletedGameRepository.EXCEPTION_MESSAGE_MUST_PASS_AT_LEAST_ONE_PLAYER)]
        public void ItRequiresMoreThanOnePlayer()
        {
            NewlyCompletedGame newlyCompletedGameThatHasNoPlayers = new NewlyCompletedGame();
            newlyCompletedGameThatHasNoPlayers.GameDefinitionId = playedGameId;
            newlyCompletedGameThatHasNoPlayers.PlayerRanks = new List<PlayerRank>();

            playedGameLogic.CreatePlayedGame(newlyCompletedGameThatHasNoPlayers);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = CompletedGameRepository.EXCEPTION_MESSAGE_MUST_PASS_VALID_GAME_DEFINITION_ID)]
        public void ItRequiresAGame()
        {
            NewlyCompletedGame newlyCompletedGameThatHasNoGameDefinitionId = new NewlyCompletedGame();
            newlyCompletedGameThatHasNoGameDefinitionId.PlayerRanks = new List<PlayerRank>() { new PlayerRank() { PlayerId = 2, GameRank = 1 } };

            playedGameLogic.CreatePlayedGame(newlyCompletedGameThatHasNoGameDefinitionId);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = CompletedGameRepository.EXCEPTION_MESSAGE_GAME_MUST_HAVE_A_WINNER)]
        public void ItRequiresAWinner()
        {
            NewlyCompletedGame newlyCompletedGameWithoutAWinner = new NewlyCompletedGame() { GameDefinitionId = 1 };
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 2 });
            playerRanks.Add(new PlayerRank() { PlayerId = 1, GameRank = 3 });
            newlyCompletedGameWithoutAWinner.PlayerRanks = playerRanks;

            playedGameLogic.CreatePlayedGame(newlyCompletedGameWithoutAWinner);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = CompletedGameRepository.EXCEPTION_MESSAGE_GAME_MUST_NOT_HAVE_A_GAP_IN_RANKS)]
        public void ItThrowsArgumentExceptionIfMissingARankInbetweenTwoOtherRanks()
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
