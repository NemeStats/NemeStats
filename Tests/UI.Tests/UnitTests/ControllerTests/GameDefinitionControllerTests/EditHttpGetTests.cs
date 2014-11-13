using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class EditHttpGetTests : GameDefinitionControllerTestBase
    {
        [Test]
        public void ItReturnsAnEditView()
        {
            int gameDefinitionId = 111;
            ViewResult viewResult = gameDefinitionControllerPartialMock.Edit(gameDefinitionId, currentUser) as ViewResult;

            Assert.AreEqual(MVC.GameDefinition.Views.Edit, viewResult.ViewName);
        }

        [Test]
        public void ItSetsTheGameDefinitionOnTheView()
        {
            GameDefinitionSummary gameDefinitionSummary = new GameDefinitionSummary()
            {
                Id = 135
            };
            gameDefinitionRetrieverMock.Expect(mock => mock.GetGameDefinitionDetails(gameDefinitionSummary.Id, 0))
                .Repeat.Once()
                .Return(gameDefinitionSummary);

            ViewResult viewResult = gameDefinitionControllerPartialMock.Edit(gameDefinitionSummary.Id, currentUser) as ViewResult;

            Assert.AreEqual(gameDefinitionSummary, viewResult.ViewData.Model);
        }

        [Test]
        public void ItReturnsABadRequestHttpStatusCodeIfNoIdIsPassed()
        {
            int? nullGameId = null;
            HttpStatusCodeResult statusCodeResult = gameDefinitionControllerPartialMock.Edit(nullGameId, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, statusCodeResult.StatusCode);
        }

        [Test]
        public void ItReturnsANotFoundHttpStatusCodeIfNoGameDefinitionIsFound()
        {
            int gameDefinitionId = -1;
            gameDefinitionRetrieverMock.Expect(mock => mock.GetGameDefinitionDetails(gameDefinitionId, 0))
                .Repeat.Once()
                .Throw(new KeyNotFoundException());
            HttpStatusCodeResult statusCodeResult = gameDefinitionControllerPartialMock.Edit(gameDefinitionId, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, statusCodeResult.StatusCode);
        }
    }
}
