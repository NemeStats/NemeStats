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
            GameDefinition gameDefinition = new GameDefinition()
            {
                Name = "game definition name"
            };

            gameDefinitionControllerPartialMock.Create(gameDefinition, currentUser);

            gameDefinitionCreatorMock.AssertWasCalled(mock => mock.Save(gameDefinition, currentUser));
        }

        [Test]
        public void ItRedirectsToTheGamingGroupIndexAndGameDefinitionSectionAfterSaving()
        {
            string baseUrl = "base url";
            string expectedUrl = baseUrl + "#" + GamingGroupController.SECTION_ANCHOR_GAMEDEFINITIONS;
            GameDefinition gameDefinition = new GameDefinition();
            urlHelperMock.Expect(mock => mock.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name))
                    .Return(baseUrl);

            RedirectResult redirectResult = gameDefinitionControllerPartialMock.Create(gameDefinition, currentUser) as RedirectResult;

            Assert.AreEqual(expectedUrl, redirectResult.Url);
        }
    }
}
