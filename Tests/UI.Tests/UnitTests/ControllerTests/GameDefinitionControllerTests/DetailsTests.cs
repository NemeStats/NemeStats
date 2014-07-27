using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
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
    public class DetailsTests : GameDefinitionControllerTestBase
    {
        private int gameDefinitionId = 1;

        [Test]
        public void ItReturnsTheDetailsView()
        {
            gameDefinitionRepository.Expect(repo => repo.GetGameDefinition(
                Arg<int>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));

            ViewResult viewResult = gameDefinitionControllerPartialMock.Details(gameDefinitionId, currentUser) as ViewResult;

            Assert.AreEqual(MVC.GameDefinition.Views.Details, viewResult.ViewName);
        }

        [Test]
        public void ItReturnsTheSpecifiedGameDefinitionOnTheView()
        {
            GameDefinition gameDefinition = new GameDefinition();
            gameDefinitionRepository.Expect(repo => repo.GetGameDefinition(
                gameDefinitionId,
                currentUser))
                   .Repeat.Once()
                   .Return(gameDefinition);

            ViewResult viewResult = gameDefinitionControllerPartialMock.Details(gameDefinitionId, currentUser) as ViewResult;
            GameDefinition actualGameDefinition = (GameDefinition)viewResult.ViewData.Model;

            Assert.AreEqual(gameDefinition, actualGameDefinition);
        }

        [Test]
        public void ItReturnsABadHttpRequestStatusCodeIfTheIdIsNull()
        {
            HttpStatusCodeResult result = gameDefinitionControllerPartialMock.Details(null, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public void ItReturnsAnHttpNotFoundStatusCodeIfTheGameDefinitionIsNotFound()
        {
            gameDefinitionRepository.Expect(repo => repo.GetGameDefinition(
                Arg<int>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything))
                .Throw(new KeyNotFoundException());

            HttpStatusCodeResult result = gameDefinitionControllerPartialMock.Details(999999, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Test]
        public void ItReturnsAnHttpNotAuthorizedStatusCodeIfUserIsNotAuthorizedToViewTheGameDefintion()
        {
            gameDefinitionRepository.Expect(repo => repo.GetGameDefinition(
                Arg<int>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything))
                .Throw(new UnauthorizedAccessException());

            HttpStatusCodeResult result = gameDefinitionControllerPartialMock.Details(999999, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }
    }
}
