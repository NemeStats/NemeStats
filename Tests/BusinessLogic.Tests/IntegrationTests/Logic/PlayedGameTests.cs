using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using Models.Logic;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Tests.IntegrationTests.Logic
{
    [TestFixture]
    public class PlayedGameTests
    {
        private NerdScorekeeperDbContext dbContext = null;
        CompletedGame playedGameLogic = null;
        private int playedGameId = 2;

        [SetUp]
        public void SetUp()
        {
            dbContext = new NerdScorekeeperDbContext();
            playedGameLogic = new CompletedGame(dbContext);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ItRequiresANewPlayedGameNotAnExistingOne()
        {
            PlayedGame playedGameThatIsAlreadyInTheDatabase = new PlayedGame(){ Id = 2};

            PlayedGame playedGame = playedGameLogic.CreatePlayedGame(playedGameThatIsAlreadyInTheDatabase);
        }

        [Test]
        public void ItCreatesATwoPlayerGame()
        {
            PlayedGame newPlayedGame = new PlayedGame() { NumberOfPlayers = 2, GameDefinitionId = playedGameId };

            

            PlayedGame playedGameWithId = playedGameLogic.CreatePlayedGame(newPlayedGame);

            Assert.AreNotEqual(newPlayedGame.Id, playedGameWithId);
        }
    }
}
