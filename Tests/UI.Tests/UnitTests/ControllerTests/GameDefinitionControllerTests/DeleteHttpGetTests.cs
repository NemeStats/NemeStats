using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class DeleteHttpGetTests : GameDefinitionControllerTestBase
    {
        [Test]
        public void ItReturnsAnEditView()
        {
            int gameDefinitionId = 15;
            gameDefinitionRepository.Expect(mock => mock.GetGameDefinition(gameDefinitionId, dbContext, userContext))
                .Repeat.Once()
                .Return(new GameDefinition());
            ViewResult viewResult = gameDefinitionControllerPartialMock.Delete(gameDefinitionId, userContext) as ViewResult;

            Assert.AreEqual(MVC.GameDefinition.Views.Delete, viewResult.ViewName);
        }

        [Test]
        public void ItSetsTheGameDefinitionOnTheView()
        {
            GameDefinition gameDefinition = new GameDefinition()
            {
                Id = 151
            };
            gameDefinitionRepository.Expect(mock => mock.GetGameDefinition(gameDefinition.Id, dbContext, userContext))
                .Repeat.Once()
                .Return(gameDefinition);

            ViewResult viewResult = gameDefinitionControllerPartialMock.Delete(gameDefinition.Id, userContext) as ViewResult;

            Assert.AreEqual(gameDefinition, viewResult.ViewData.Model);
        }

        [Test]
        public void ItReturnsABadRequestHttpStatusCodeIfNoIdIsPassed()
        {
            HttpStatusCodeResult httpResult = gameDefinitionControllerPartialMock.Delete(null, userContext) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, httpResult.StatusCode);
        }

        [Test]
        public void ItReturnsANotFoundHttpStatusCodeIfNoGameDefinitionIsFound()
        {
            gameDefinitionRepository.Expect(mock => mock.GetGameDefinition(-1, dbContext, userContext))
                .Throw(new KeyNotFoundException());
            HttpStatusCodeResult httpResult = gameDefinitionControllerPartialMock.Delete(-1, userContext) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, httpResult.StatusCode);
        }

        [Test]
        public void ItReturnsAnUnauthorizedAccessHttpStatusCodeIfNoGameDefinitionIsFound()
        {
            gameDefinitionRepository.Expect(mock => mock.GetGameDefinition(1, dbContext, userContext))
                .Throw(new UnauthorizedAccessException());
            HttpStatusCodeResult httpResult = gameDefinitionControllerPartialMock.Delete(1, userContext) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.Unauthorized, httpResult.StatusCode);
        }
    }
}
