using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class EditHttpPostTests : GameDefinitionControllerTestBase
    {
        [Test]
        public void ItStaysOnTheEditPageIfValidationFails()
        {
            GameDefinition gameDefinition = new GameDefinition();
            gameDefinitionControllerPartialMock.ModelState.AddModelError("key", "message");

            ViewResult viewResult = gameDefinitionControllerPartialMock.Edit(gameDefinition, currentUser) as ViewResult;

            Assert.AreEqual(MVC.GameDefinition.Views.Edit, viewResult.ViewName);
        }

        [Test]
        public void ItReloadsTheGameDefinitionIfValidationFails()
        {
            GameDefinition gameDefinition = new GameDefinition();
            gameDefinitionControllerPartialMock.ModelState.AddModelError("key", "message");

            ViewResult viewResult = gameDefinitionControllerPartialMock.Edit(gameDefinition, currentUser) as ViewResult;

            Assert.AreSame(gameDefinition, viewResult.Model);
        }

        [Test]
        public void ItSavesTheGameDefinitionIfValidationPasses()
        {
            GameDefinition gameDefinition = new GameDefinition();

            gameDefinitionControllerPartialMock.Edit(gameDefinition, currentUser);
            
            gameDefinitionRepository.AssertWasCalled(repo => dbContext.Save(gameDefinition, currentUser));
        }

        [Test]
        public void ItRedirectsToTheIndexActionAfterSaving()
        {
            GameDefinition gameDefinition = new GameDefinition();

            RedirectToRouteResult redirectToRouteResult = gameDefinitionControllerPartialMock.Edit(gameDefinition, currentUser) as RedirectToRouteResult;

            Assert.AreEqual(MVC.GameDefinition.ActionNames.Index, redirectToRouteResult.RouteValues["action"]);
        }
    }
}
