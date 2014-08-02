using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class CreateHttpPostTests : PlayerControllerTestBase
    {
        [Test]
        public void ItSavesThePlayer()
        {
            Player player = new Player();
            dataContextMock.Expect(mock => mock.Save<Player>(player, currentUser));

            playerController.Create(player, currentUser);
        }

        [Test]
        public void ItRedirectsToTheIndexViewAfterSaving()
        {
            RedirectToRouteResult result = playerController.Create(new Player(), currentUser) as RedirectToRouteResult;

            Assert.AreEqual(MVC.Player.ActionNames.Index, result.RouteValues["action"]);
        }

        [Test]
        public void ItRemainsOnTheCreateViewIfValidationFails()
        {
            playerController.ModelState.AddModelError("key", "message");
            ViewResult result = playerController.Create(new Player(), currentUser) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Index, result.ViewName);
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
