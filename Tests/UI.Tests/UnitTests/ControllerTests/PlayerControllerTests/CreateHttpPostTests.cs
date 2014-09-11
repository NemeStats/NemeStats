using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class CreateHttpPostTests : PlayerControllerTestBase
    {
        [Test]
        public void ItSavesThePlayer()
        {
            Player player = new Player()
            {
                Name = "player name"
            };

            playerController.Create(player, currentUser);

            playerCreatorMock.AssertWasCalled(mock => mock.Save(player, currentUser));
        }

        [Test]
        public void ItRedirectsToTheGamingGroupIndexAndPlayerSectionAfterSaving()
        {
            string baseUrl = "base url";
            string expectedUrl = baseUrl + "#" + GamingGroupController.SECTION_ANCHOR_PLAYERS;
            urlHelperMock.Expect(mock => mock.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name))
                    .Return(baseUrl);

            RedirectResult redirectResult = playerController.Create(new Player(), currentUser) as RedirectResult;

            Assert.AreEqual(expectedUrl, redirectResult.Url);
        }

        [Test]
        public void ItRemainsOnTheCreateViewIfValidationFails()
        {
            playerController.ModelState.AddModelError("key", "message");
            ViewResult result = playerController.Create(new Player(), currentUser) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Create, result.ViewName);
        }

        [Test]
        public void ItPutsThePlayerOnTheViewIfValidationFails()
        {
            Player player = new Player();
            playerController.ModelState.AddModelError("key", "message");
            ViewResult result = playerController.Create(player, currentUser) as ViewResult;

            Assert.AreEqual(player, result.Model);
        }
    }
}
