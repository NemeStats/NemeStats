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
    public class CompletedGameTests
    {
        private NerdScorekeeperDbContext dbContext = null;
        CompletedGame playedGameLogic = null;
        private int playedGameId = 2;

        [SetUp]
        public void SetUp()
        {
            dbContext = MockRepository.GenerateMock<NerdScorekeeperDbContext>();
            playedGameLogic = new CompletedGame(dbContext);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ItRequiresMoreThanOnePlayer()
        {
            NewlyCompletedGame newlyCompletedGameThatHasNoPlayers = new NewlyCompletedGame();
            newlyCompletedGameThatHasNoPlayers.GameDefinitionId = playedGameId;
            newlyCompletedGameThatHasNoPlayers.PlayerRanks = new List<PlayerRank>();

            playedGameLogic.CreatePlayedGame(newlyCompletedGameThatHasNoPlayers);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ItRequiresAGame()
        {
            NewlyCompletedGame newlyCompletedGameThatHasNoGameDefinitionId = new NewlyCompletedGame();
            newlyCompletedGameThatHasNoGameDefinitionId.PlayerRanks = new List<PlayerRank>() { new PlayerRank() { PlayerId = 2, GameRank = 1 } };
            playedGameLogic.CreatePlayedGame(newlyCompletedGameThatHasNoGameDefinitionId);
        }
    }
}
