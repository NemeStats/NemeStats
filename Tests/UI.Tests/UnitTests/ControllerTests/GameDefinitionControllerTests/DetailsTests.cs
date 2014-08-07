using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
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
using UI.Controllers;
using UI.Models.GameDefinitionModels;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class DetailsTests : GameDefinitionControllerTestBase
    {
        private int gameDefinitionId = 1;
        private GameDefinition gameDefinition = new GameDefinition();
        private GameDefinitionViewModel expectedViewModel = new GameDefinitionViewModel();

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            gameDefinitionRetrieverMock.Expect(repo => repo.GetGameDefinitionDetails(
                Arg<int>.Is.Anything,
                Arg<int>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything))
                .Return(gameDefinition);
            gameDefinitionTransformation.Expect(mock => mock.Build(gameDefinition))
                .Return(expectedViewModel);
        }

        [Test]
        public void ItReturnsTheDetailsView()
        {
            ViewResult viewResult = gameDefinitionControllerPartialMock.Details(gameDefinitionId, currentUser) as ViewResult;

            Assert.AreEqual(MVC.GameDefinition.Views.Details, viewResult.ViewName);
        }

        [Test]
        public void ItReturnsTheSpecifiedGameDefinitionViewModelOnTheView()
        {
            ViewResult viewResult = gameDefinitionControllerPartialMock.Details(gameDefinitionId, currentUser) as ViewResult;

            GameDefinitionViewModel actualGameDefinitionViewModel = (GameDefinitionViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(expectedViewModel, actualGameDefinitionViewModel);
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
            gameDefinitionControllerPartialMock.gameDefinitionRetriever = MockRepository.GenerateMock<GameDefinitionRetriever>();
            gameDefinitionControllerPartialMock.gameDefinitionRetriever.Expect(mock => mock.GetGameDefinitionDetails(
                Arg<int>.Is.Anything,
                Arg<int>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything))
                .Throw(new KeyNotFoundException());

            HttpStatusCodeResult result = gameDefinitionControllerPartialMock.Details(999999, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Test]
        public void ItReturnsAnHttpNotAuthorizedStatusCodeIfUserIsNotAuthorizedToViewTheGameDefintion()
        {
            gameDefinitionControllerPartialMock.gameDefinitionRetriever = MockRepository.GenerateMock<GameDefinitionRetriever>();
            gameDefinitionControllerPartialMock.gameDefinitionRetriever.Expect(repo => repo.GetGameDefinitionDetails(
                Arg<int>.Is.Anything,
                Arg<int>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything))
                .Throw(new UnauthorizedAccessException());

            HttpStatusCodeResult result = gameDefinitionControllerPartialMock.Details(999999, currentUser) as HttpStatusCodeResult;
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }
    }
}
