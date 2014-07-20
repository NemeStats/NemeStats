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
    public class CreateHttpPostTests : GameDefinitionControllerTestBase
    {
        [Test]
        public void ItStaysOnTheCreatePageIfValidationFails()
        {
            gameDefinitionControllerPartialMock.ModelState.AddModelError("key", "message");

            ViewResult actionResult = gameDefinitionControllerPartialMock.Create(null, currentUser) as ViewResult;

            Assert.AreEqual(MVC.GameDefinition.Views.Create, actionResult.ViewName);
        }

        [Test]
        public void ItReloadsTheCurrentGameDefinitionIfValidationFails()
        {
            GameDefinition gameDefinition = new GameDefinition();
            gameDefinitionControllerPartialMock.ModelState.AddModelError("key", "message");

            ViewResult actionResult = gameDefinitionControllerPartialMock.Create(gameDefinition, currentUser) as ViewResult;
            GameDefinition actualViewModel = (GameDefinition)actionResult.ViewData.Model;

            Assert.AreSame(gameDefinition, actualViewModel);
        }

        [Test]
        public void ItSavesTheGameDefinitionIfValidationPasses()
        {
            GameDefinition gameDefinition = new GameDefinition();

            gameDefinitionControllerPartialMock.Create(gameDefinition, currentUser);

            gameDefinitionRepository.AssertWasCalled(repo => repo.Save(gameDefinition, currentUser));
        }

        [Test]
        public void ItRedirectsToTheIndexActionAfterSaving()
        {
            GameDefinition gameDefinition = new GameDefinition();

            RedirectToRouteResult redirectToRouteResult = gameDefinitionControllerPartialMock.Create(gameDefinition, currentUser) as RedirectToRouteResult;

            Assert.AreEqual(MVC.GameDefinition.ActionNames.Index, redirectToRouteResult.RouteValues["action"]);
        }
    }
}
