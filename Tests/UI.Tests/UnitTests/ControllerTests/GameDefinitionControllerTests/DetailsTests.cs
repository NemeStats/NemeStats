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
using System.Net;
using System.Web.Mvc;
using UI.Models.GameDefinitionModels;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class DetailsTests : GameDefinitionControllerTestBase
    {
        private int gameDefinitionId = 1;
        private GameDefinitionSummary gameDefinitionSummary;
        private readonly GameDefinitionDetailsViewModel2 expectedViewModel = new GameDefinitionDetailsViewModel2();

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            gameDefinitionSummary = new GameDefinitionSummary()
            {
                PlayedGames = new List<PlayedGame>(),
                GamingGroupId = currentUser.CurrentGamingGroupId
            };

            autoMocker.Get<IGameDefinitionRetriever>().Expect(repo => repo.GetGameDefinitionDetails(
                Arg<int>.Is.Anything,
                Arg<int>.Is.Anything))
                .Return(gameDefinitionSummary);
            autoMocker.Get<IGameDefinitionDetailsViewModelBuilder>().Expect(mock => mock.Build(gameDefinitionSummary, currentUser))
                .Return(expectedViewModel);
        }

        [Test]
        public void ItReturnsTheDetailsView()
        {
            var viewResult = autoMocker.ClassUnderTest.Details(gameDefinitionId, currentUser) as ViewResult;

            Assert.AreEqual(MVC.GameDefinition.Views.Details, viewResult.ViewName);
        }

        [Test]
        public void ItReturnsTheSpecifiedGameDefinitionViewModelOnTheView()
        {
            var viewResult = autoMocker.ClassUnderTest.Details(gameDefinitionId, currentUser) as ViewResult;

            var actualGameDefinitionViewModel = (GameDefinitionDetailsViewModel2)viewResult.ViewData.Model;
            Assert.AreEqual(expectedViewModel, actualGameDefinitionViewModel);
        }

        [Test]
        public void ItReturnsABadHttpRequestStatusCodeIfTheIdIsNull()
        {
            var result = autoMocker.ClassUnderTest.Details(null, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public void ItReturnsAnHttpNotFoundStatusCodeIfTheGameDefinitionIsNotFound()
        {
            autoMocker.Get<IGameDefinitionRetriever>().BackToRecord(BackToRecordOptions.All);
            autoMocker.Get<IGameDefinitionRetriever>().Replay();
            autoMocker.Get<IGameDefinitionRetriever>().Expect(mock => mock.GetGameDefinitionDetails(
                Arg<int>.Is.Anything,
                Arg<int>.Is.Anything))
                .Throw(new KeyNotFoundException());

            var result = autoMocker.ClassUnderTest.Details(999999, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Test]
        public void ItReturnsAnHttpNotAuthorizedStatusCodeIfUserIsNotAuthorizedToViewTheGameDefintion()
        {
            autoMocker.Get<IGameDefinitionRetriever>().BackToRecord(BackToRecordOptions.All);
            autoMocker.Get<IGameDefinitionRetriever>().Replay();
            autoMocker.Get<IGameDefinitionRetriever>().Expect(repo => repo.GetGameDefinitionDetails(
                Arg<int>.Is.Anything,
                Arg<int>.Is.Anything))
                .Throw(new UnauthorizedAccessException());

            var result = autoMocker.ClassUnderTest.Details(999999, currentUser) as HttpStatusCodeResult;
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }
    }
}
