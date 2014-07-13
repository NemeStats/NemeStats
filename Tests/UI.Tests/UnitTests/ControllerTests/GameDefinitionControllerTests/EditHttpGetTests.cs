using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
            ViewResult viewResult = gameDefinitionControllerPartialMock.Edit(gameDefinitionId, userContext) as ViewResult;

            Assert.AreEqual(MVC.GameDefinition.Views.Edit, viewResult.ViewName);
        }

        [Test]
        public void ItSetsTheGameDefinitionOnTheView()
        {
            GameDefinition gameDefinition = new GameDefinition()
            {
                Id = 135
            };
            gameDefinitionRepository.Expect(mock => mock.GetGameDefinition(gameDefinition.Id, userContext))
                .Repeat.Once()
                .Return(gameDefinition);

            ViewResult viewResult = gameDefinitionControllerPartialMock.Edit(gameDefinition.Id, userContext) as ViewResult;

            Assert.AreEqual(gameDefinition, viewResult.ViewData.Model);
        }

        [Test]
        public void ItReturnsABadRequestHttpStatusCodeIfNoIdIsPassed()
        {
            int? nullGameId = null;
            HttpStatusCodeResult statusCodeResult = gameDefinitionControllerPartialMock.Edit(nullGameId, userContext) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, statusCodeResult.StatusCode);
        }

        [Test]
        public void ItReturnsANotFoundHttpStatusCodeIfNoGameDefinitionIsFound()
        {
            int gameDefinitionId = -1;
            gameDefinitionRepository.Expect(mock => mock.GetGameDefinition(gameDefinitionId, userContext))
                .Repeat.Once()
                .Throw(new KeyNotFoundException());
            HttpStatusCodeResult statusCodeResult = gameDefinitionControllerPartialMock.Edit(gameDefinitionId, userContext) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, statusCodeResult.StatusCode);
        }
    }
}
