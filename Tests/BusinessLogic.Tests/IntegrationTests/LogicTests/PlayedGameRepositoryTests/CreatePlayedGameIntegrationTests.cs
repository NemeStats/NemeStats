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

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.PlayedGameRepositoryTests
{
    [TestFixture]
    public class CreatePlayedGameIntegrationTests : IntegrationTestBase
    {
        [Test]
        public void ItCreatesATwoPlayerPlayedGameAndSetsTheNumberOfPlayers()
        {
            PlayedGame playedGameFromTheDatabase = dbContext.PlayedGames.Find(testPlayedGames[0].Id);

            Assert.IsTrue(playedGameFromTheDatabase.NumberOfPlayers == 2);
        }

        //TODO need more integration tests, but have been looking at the database manually and it looks OK.
    }
}
