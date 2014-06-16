using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    [TestFixture]
    public class CreateWithNoParametersTests
    {
        private NemeStatsDbContext dbContexMock;
        private UI.Controllers.PlayedGameController playedGameController;
        private PlayedGameLogic playedGameLogic;
        private PlayerLogic playerLogic;

        [TestFixtureSetUp]
        public void SetUp()
        {
            
        }

        [SetUp]
        public void TestSetUp()
        {
            dbContexMock = MockRepository.GenerateMock<NemeStatsDbContext>();
            playedGameLogic = MockRepository.GenerateMock<PlayedGameLogic>();
            playerLogic = MockRepository.GenerateMock<PlayerLogic>();
            playedGameController = new Controllers.PlayedGameController(dbContexMock, playedGameLogic, playerLogic);
            dbContexMock.Expect(context => context.GameDefinitions).Repeat.Any().Return(MockRepository.GenerateMock<DbSet<GameDefinition>>());
        }

        //TODO this is also tested in CreateTests. Should there be an interface? Static method? Base Class for the tests?
        [Test]
        public void ItAddsAllActivePlayersToTheViewBag()
        {
            int playerId = 1938;
            string playerName = "Herb";
            List<Player> allPlayers = new List<Player>() { new Player() { Id = playerId, Name = playerName } };

            playerLogic.Expect(x => x.GetAllPlayers(true)).Repeat.Once().Return(allPlayers);

            playedGameController.Create();

            playerLogic.VerifyAllExpectations();

            List<SelectListItem> selectListItems = playedGameController.ViewBag.Players;
            Assert.True(selectListItems.All(x => x.Value == playerId.ToString() && x.Text == playerName));
        }

        [Test]
        public void ItLoadsTheCreateView()
        {
            playerLogic.Expect(x => x.GetAllPlayers(true)).Repeat.Once().Return(new List<Player>());

            ViewResult result = playedGameController.Create() as ViewResult;

            Assert.AreEqual(MVC.PlayedGame.Views.Create, result.ViewName);
        }
    }
}
