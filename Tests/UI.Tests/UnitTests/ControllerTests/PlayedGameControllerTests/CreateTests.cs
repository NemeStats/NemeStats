using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    [TestFixture]
    public class CreateTests
    {
        private NemeStatsDbContext dbContexMock;
        private UI.Controllers.PlayedGameController playedGameController;
        private PlayedGameLogic playedGameLogic;
        private PlayerLogic playerLogic;

        [TestFixtureSetUp]
        public void SetUp()
        {
            dbContexMock = MockRepository.GenerateMock<NemeStatsDbContext>();
            playedGameLogic = MockRepository.GenerateMock<PlayedGameLogic>();
            playerLogic = MockRepository.GenerateMock<PlayerLogic>();
        }

        [SetUp]
        public void TestSetUp()
        {
            playedGameController = new Controllers.PlayedGameController(dbContexMock, playedGameLogic, playerLogic);
        }

        [Test]
        public void ItRemainsOnTheCreatePageIfTheModelIsNotValid()
        {
            playerLogic.Expect(x => x.GetAllPlayers(true)).Repeat.Once().Return(new List<Player>());
            playedGameController.ModelState.AddModelError("Test error", "this is a test error to make model state invalid");

            ViewResult result = playedGameController.Create(new NewlyCompletedGame()) as ViewResult;

            Assert.AreEqual(MVC.PlayedGame.Views.Create, result.ViewName);
        }

        [Test]
        public void ItAddsAllActivePlayersToTheViewBagIfRemainingOnTheCreatePage()
        {
            int playerId = 1938;
            string playerName = "Herb";
            List<Player> allPlayers = new List<Player>() { new Player() { Id = playerId, Name = playerName } };

            playerLogic.Expect(x => x.GetAllPlayers(true)).Repeat.Once().Return(allPlayers);
            playedGameController.ModelState.AddModelError("Test error", "this is a test error to make model state invalid");

            playedGameController.Create(new NewlyCompletedGame());

            playerLogic.VerifyAllExpectations();

            List<SelectListItem> selectListItems = playedGameController.ViewBag.Players;
            Assert.True(selectListItems.All(x => x.Value == playerId.ToString() && x.Text == playerName));
        }

        [Test]
        public void ItLoadsTheIndexPageIfSaveIsSuccessful()
        {
            NewlyCompletedGame playedGame = new NewlyCompletedGame()
            { 
                GameDefinitionId = 1, 
                PlayerRanks = new List<PlayerRank>()
            };
            playedGameLogic.Expect(x => x.CreatePlayedGame(Arg<NewlyCompletedGame>.Is.Anything)).Repeat.Once();
            RedirectToRouteResult result = playedGameController.Create(playedGame) as RedirectToRouteResult;

            Assert.AreEqual(MVC.PlayedGame.ActionNames.Index, result.RouteValues["action"]);
        }
    }
}
