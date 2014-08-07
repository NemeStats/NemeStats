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
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class DeleteHttpGetTests : GameDefinitionControllerTestBase
    {
        [Test]
        public void ItReturnsADeleteView()
        {
            int gameDefinitionId = 15;
            gameDefinitionRetrieverMock.Expect(mock => mock.GetGameDefinitionDetails(
                gameDefinitionId, 
                0, 
                currentUser))
                .Repeat.Once()
                .Return(new GameDefinition());

            ViewResult viewResult = gameDefinitionControllerPartialMock.Delete(gameDefinitionId, currentUser) as ViewResult;

            Assert.AreEqual(MVC.GameDefinition.Views.Delete, viewResult.ViewName);
        }

        [Test]
        public void ItSetsTheGameDefinitionOnTheView()
        {
            GameDefinition gameDefinition = new GameDefinition()
            {
                Id = 151
            };
            gameDefinitionRetrieverMock.Expect(mock => mock.GetGameDefinitionDetails(
                gameDefinition.Id,
                0,
                currentUser))
                .Repeat.Once()
                .Return(gameDefinition);

            ViewResult viewResult = gameDefinitionControllerPartialMock.Delete(gameDefinition.Id, currentUser) as ViewResult;

            Assert.AreEqual(gameDefinition, viewResult.ViewData.Model);
        }

        [Test]
        public void ItReturnsABadRequestHttpStatusCodeIfNoIdIsPassed()
        {
            HttpStatusCodeResult httpResult = gameDefinitionControllerPartialMock.Delete(null, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, httpResult.StatusCode);
        }

        [Test]
        public void ItReturnsANotFoundHttpStatusCodeIfNoGameDefinitionIsFound()
        {
            gameDefinitionRetrieverMock.Expect(mock => mock.GetGameDefinitionDetails(
                -1, 
                0,
                currentUser))
                .Throw(new KeyNotFoundException());
            HttpStatusCodeResult httpResult = gameDefinitionControllerPartialMock.Delete(-1, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, httpResult.StatusCode);
        }

        [Test]
        public void ItReturnsAnUnauthorizedAccessHttpStatusCodeIfTheUserIsNotAuthorized()
        {
            gameDefinitionRetrieverMock.Expect(mock => mock.GetGameDefinitionDetails(
                1, 
                0, 
                currentUser))
                .Throw(new UnauthorizedAccessException());
            HttpStatusCodeResult httpResult = gameDefinitionControllerPartialMock.Delete(1, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.Unauthorized, httpResult.StatusCode);
        }

        [Test]
        public void ItDoesntReturnAnyPlayedGames()
        {
            gameDefinitionRetrieverMock.Expect(mock => mock.GetGameDefinitionDetails(
                1,
                0,
                currentUser))
                .Throw(new UnauthorizedAccessException());
            HttpStatusCodeResult httpResult = gameDefinitionControllerPartialMock.Delete(1, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.Unauthorized, httpResult.StatusCode);
        }
    }
}
