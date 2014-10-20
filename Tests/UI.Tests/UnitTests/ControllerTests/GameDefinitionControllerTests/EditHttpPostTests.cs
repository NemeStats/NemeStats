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

            dataContextMock.AssertWasCalled(mock => mock.Save(gameDefinition, currentUser));
        }

        [Test]
        public void ItRedirectsToTheGamingGroupIndexAndGameDefinitionsSectionAfterSaving()
        {
            GameDefinition gameDefinition = new GameDefinition();
            string baseUrl = "base url";
            string expectedUrl = baseUrl + "#" + GamingGroupController.SECTION_ANCHOR_GAMEDEFINITIONS;
            urlHelperMock.Expect(mock => mock.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name))
                    .Return(baseUrl);

            RedirectResult redirectResult = gameDefinitionControllerPartialMock.Edit(gameDefinition, currentUser) as RedirectResult;

            Assert.AreEqual(expectedUrl, redirectResult.Url);
        }
    }
}
