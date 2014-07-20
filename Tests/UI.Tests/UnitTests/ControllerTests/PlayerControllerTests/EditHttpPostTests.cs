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
    public class EditHttpPostTests : PlayerControllerTestBase
    {
        [Test]
        public void ItRedirectsToTheIndexAction()
        {
            RedirectToRouteResult result = playerController.Edit(new Player(), currentUser) as RedirectToRouteResult;

            Assert.AreEqual(MVC.Player.ActionNames.Index, result.RouteValues["action"]);
        }

        [Test]
        public void ItRemainsOnTheEditViewIfValidationFails()
        {
            playerController.ModelState.AddModelError("key", "message");

            ViewResult result = playerController.Edit(new Player(), currentUser) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Edit, result.ViewName);
        }

        [Test]
        public void ItPutsThePlayerOnTheViewIfValidationFails()
        {
            Player player = new Player();
            playerController.ModelState.AddModelError("key", "message");

            ViewResult result = playerController.Edit(player, currentUser) as ViewResult;

            Assert.AreEqual(player, result.Model);
        }

        [Test]
        public void ItSavesThePlayer()
        {
            Player player = new Player();

            playerController.Edit(player, currentUser);

            playerRepositoryMock.AssertWasCalled(mock => mock.Save(player, currentUser));
        }
    }
}
