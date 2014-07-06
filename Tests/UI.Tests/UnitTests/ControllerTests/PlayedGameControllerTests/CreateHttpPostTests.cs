using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    [TestFixture]
    public class CreateHttpPostTests : PlayedGameControllerTestBase
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
            playerLogicMock.Expect(x => x.GetAllPlayers(true, testUserName)).Repeat.Once().Return(new List<Player>());
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

            playerLogicMock.Expect(x => x.GetAllPlayers(true, testUserName)).Repeat.Once().Return(allPlayers);
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
            UserContext user = new UserContext();
            playedGameLogicMock.Expect(x => x.CreatePlayedGame(Arg<NewlyCompletedGame>.Is.Anything, Arg<string>.Is.Anything)).Repeat.Once();
            RedirectToRouteResult result = playedGameControllerPartialMock.Create(playedGame) as RedirectToRouteResult;

            Assert.AreEqual(MVC.PlayedGame.ActionNames.Index, result.RouteValues["action"]);
        }

        [Test]
        public void ItSavesTheNewGame()
        {
            NewlyCompletedGame newlyCompletedGame = new NewlyCompletedGame()
            {
                GameDefinitionId = 1,
                PlayerRanks = new List<PlayerRank>()
            };

            playedGameControllerPartialMock.Create(newlyCompletedGame);

            playedGameLogicMock.AssertWasCalled(mock => mock.CreatePlayedGame(Arg<NewlyCompletedGame>.Is.Equal(newlyCompletedGame), 
                Arg<string>.Is.Anything));
        }

        [Test]
        public void ItMakesTheRequestForTheCurrentUser()
        {
            NewlyCompletedGame newlyCompletedGame = new NewlyCompletedGame()
            {
                GameDefinitionId = 1,
                PlayerRanks = new List<PlayerRank>()
            };
            UserContext userContext = new UserContext()
            {
                ApplicationUserId = "1"
            };

            userContextBuilder.Expect(builder => builder.GetUserContext(Arg<string>.Is.Anything, Arg<NemeStatsDbContext>.Is.Anything))
                .Repeat.Once()
                .Return(userContext);

            playedGameControllerPartialMock.Create(newlyCompletedGame);

            playedGameLogicMock.AssertWasCalled(logic => logic.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Is.Anything, 
                Arg<string>.Is.Equal(testUserName)));
        }
    }
}
