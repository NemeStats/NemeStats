using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    [TestFixture]
    public class CreateHttpGetTests : PlayedGameControllerTestBase
    {
        [Test]
        public void ItAddsAllActivePlayersToTheViewBag()
        {
            int playerId = 1938;
            string playerName = "Herb";
            List<Player> allPlayers = new List<Player>() { new Player() { Id = playerId, Name = playerName } };

            playerLogicMock.Expect(x => x.GetAllPlayers(true, currentUser)).Repeat.Once().Return(allPlayers);

            playedGameController.Create(currentUser);

            playerLogicMock.VerifyAllExpectations();

            List<SelectListItem> selectListItems = playedGameController.ViewBag.Players;
            Assert.True(selectListItems.All(x => x.Value == playerId.ToString() && x.Text == playerName));
        }

        [Test]
        public void ItLoadsTheCreateView()
        {
            playerLogicMock.Expect(x => x.GetAllPlayers(true, currentUser)).Repeat.Once().Return(new List<Player>());

            ViewResult result = playedGameController.Create(currentUser) as ViewResult;

            Assert.AreEqual(MVC.PlayedGame.Views.Create, result.ViewName);
        }
    }
}
