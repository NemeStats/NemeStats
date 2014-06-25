using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    [TestFixture]
    public class CreateHttpPostTests : TestBase
    {
        [SetUp]
        public override void TestSetUp()
        {
            base.TestSetUp();
            dbContexMock.Expect(context => context.GameDefinitions).Repeat.Any().Return(MockRepository.GenerateMock<DbSet<GameDefinition>>());
        }

        [Test]
        public void ItRemainsOnTheCreatePageIfTheModelIsNotValid()
        {
            ViewResult expectedViewResult = new ViewResult();
            playerLogicMock.Expect(x => x.GetAllPlayers(true)).Repeat.Once().Return(new List<Player>());
            playedGameControllerPartialMock.Expect(controller => controller.Create())
                    .Repeat.Once()
                    .Return(expectedViewResult);

            playedGameControllerPartialMock.ModelState.AddModelError("Test error", "this is a test error to make model state invalid");

            ViewResult actualResult = playedGameControllerPartialMock.Create(new NewlyCompletedGame()) as ViewResult;

            Assert.AreSame(expectedViewResult, actualResult);
        }

        [Test]
        public void ItAddsAllActivePlayersToTheViewBagIfRemainingOnTheCreatePage()
        {
            int playerId = 1938;
            string playerName = "Herb";
            List<Player> allPlayers = new List<Player>() { new Player() { Id = playerId, Name = playerName } };

            playerLogicMock.Expect(x => x.GetAllPlayers(true)).Repeat.Once().Return(allPlayers);
            playedGameController.ModelState.AddModelError("Test error", "this is a test error to make model state invalid");

            playedGameController.Create(new NewlyCompletedGame());

            playerLogicMock.VerifyAllExpectations();

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
            playedGameLogicMock.Expect(x => x.CreatePlayedGame(Arg<NewlyCompletedGame>.Is.Anything)).Repeat.Once();
            RedirectToRouteResult result = playedGameController.Create(playedGame) as RedirectToRouteResult;

            Assert.AreEqual(MVC.PlayedGame.ActionNames.Index, result.RouteValues["action"]);
        }

        [Test]
        public void ItSavesTheNewGame()
        {
            NewlyCompletedGame playedGame = new NewlyCompletedGame()
            {
                GameDefinitionId = 1,
                PlayerRanks = new List<PlayerRank>()
            };

            playedGameController.Create(playedGame);

            playedGameLogicMock.AssertWasCalled(mock => mock.CreatePlayedGame(playedGame));
        }
    }
}
