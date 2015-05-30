#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UI.Controllers;
using UI.Models.GameDefinitionModels;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class DetailsTests : GameDefinitionControllerTestBase
    {
        private int gameDefinitionId = 1;
        private GameDefinitionSummary gameDefinitionSummary;
        private readonly GameDefinitionDetailsViewModel expectedViewModel = new GameDefinitionDetailsViewModel();

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            gameDefinitionSummary = new GameDefinitionSummary()
            {
                PlayedGames = new List<PlayedGame>(),
                GamingGroupId = currentUser.CurrentGamingGroupId.Value
            };

            gameDefinitionRetrieverMock.Expect(repo => repo.GetGameDefinitionDetails(
                Arg<int>.Is.Anything,
                Arg<int>.Is.Anything))
                .Return(gameDefinitionSummary);
            gameDefinitionTransformationMock.Expect(mock => mock.Build(gameDefinitionSummary, currentUser))
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

            GameDefinitionDetailsViewModel actualGameDefinitionViewModel = (GameDefinitionDetailsViewModel)viewResult.ViewData.Model;
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
            gameDefinitionControllerPartialMock.gameDefinitionRetriever = MockRepository.GenerateMock<IGameDefinitionRetriever>();
            gameDefinitionControllerPartialMock.gameDefinitionRetriever.Expect(mock => mock.GetGameDefinitionDetails(
                Arg<int>.Is.Anything,
                Arg<int>.Is.Anything))
                .Throw(new KeyNotFoundException());

            HttpStatusCodeResult result = gameDefinitionControllerPartialMock.Details(999999, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Test]
        public void ItReturnsAnHttpNotAuthorizedStatusCodeIfUserIsNotAuthorizedToViewTheGameDefintion()
        {
            gameDefinitionControllerPartialMock.gameDefinitionRetriever = MockRepository.GenerateMock<IGameDefinitionRetriever>();
            gameDefinitionControllerPartialMock.gameDefinitionRetriever.Expect(repo => repo.GetGameDefinitionDetails(
                Arg<int>.Is.Anything,
                Arg<int>.Is.Anything))
                .Throw(new UnauthorizedAccessException());

            HttpStatusCodeResult result = gameDefinitionControllerPartialMock.Details(999999, currentUser) as HttpStatusCodeResult;
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Test]
        public void ItPutsTheRecentGamesMessageOnTheViewBag()
        {
            string expectedMessage = "expected message";
            showingXResultsMessageBuilderMock.Expect(mock => mock.BuildMessage(
                GameDefinitionController.NUMBER_OF_RECENT_GAMES_TO_SHOW,
                gameDefinitionSummary.PlayedGames.Count))
                    .Return(expectedMessage);
                
            gameDefinitionControllerPartialMock.Details(gameDefinitionId, currentUser);

            Assert.AreEqual("Recently Played Games " + expectedMessage, gameDefinitionControllerPartialMock.ViewBag.PlayedGamesPartialPanelTitle);
        }
    }
}
