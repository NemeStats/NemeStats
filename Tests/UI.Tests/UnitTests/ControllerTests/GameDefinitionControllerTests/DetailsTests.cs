﻿#region LICENSE
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
using BusinessLogic.Logic.Players;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NemeStats.TestingHelpers.NemeStatsTestingExtensions;
using Shouldly;
using UI.Models.GameDefinitionModels;
using UI.Transformations;
using BusinessLogic.Exceptions;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class DetailsTests : GameDefinitionControllerTestBase
    {
        private int _gameDefinitionId = 1;
        private GameDefinitionSummary _expectedGameDefinitionSummary;
        private readonly GameDefinitionDetailsViewModel _expectedViewModel = new GameDefinitionDetailsViewModel();
        private readonly int _playerId1 = 1;
        private readonly int _playerId2 = 2;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _expectedGameDefinitionSummary = new GameDefinitionSummary
            {
                PlayedGames = new List<PlayedGame>(),
                GamingGroupId = currentUser.CurrentGamingGroupId.Value,
                PlayerWinRecords = new List<PlayerWinRecord>
                {
                    new PlayerWinRecord
                    {
                        PlayerId = _playerId1
                    },
                    new PlayerWinRecord
                    {
                        PlayerId = _playerId2
                    }
                }
            };

            autoMocker.Get<IGameDefinitionRetriever>().Expect(repo => repo.GetGameDefinitionDetails(
                Arg<int>.Is.Anything,
                Arg<int>.Is.Anything))
                .Return(_expectedGameDefinitionSummary);

            autoMocker.Get<IGameDefinitionDetailsViewModelBuilder>().Expect(mock => mock.Build(
                    Arg<GameDefinitionSummary>.Is.Anything, 
                    Arg<Dictionary<int, string>>.Is.Anything,
                    Arg<ApplicationUser>.Is.Anything))
                .Return(_expectedViewModel);
        }

        [Test]
        public void ItReturnsTheDetailsView()
        {
            var viewResult = autoMocker.ClassUnderTest.Details(_gameDefinitionId, currentUser) as ViewResult;

            Assert.AreEqual(MVC.GameDefinition.Views.Details, viewResult.ViewName);
        }

        [Test]
        public void ItReturnsTheSpecifiedGameDefinitionViewModelOnTheView()
        {
            //--act
            var viewResult = autoMocker.ClassUnderTest.Details(_gameDefinitionId, currentUser) as ViewResult;

            //--assert
            var args = autoMocker.Get<IGameDefinitionDetailsViewModelBuilder>()
                .GetArgumentsForCallsMadeOn(mock => mock.Build(Arg<GameDefinitionSummary>.Is.Anything, Arg<Dictionary<int, string>>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
            var actualGameDefinitionSummary = args.AssertFirstCallIsType<GameDefinitionSummary>();
            actualGameDefinitionSummary.ShouldBeSameAs(_expectedGameDefinitionSummary);
            var actualApplicationUser = args.AssertFirstCallIsType<ApplicationUser>(2);
            actualApplicationUser.ShouldBeSameAs(currentUser);
            var actualGameDefinitionViewModel = (GameDefinitionDetailsViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(_expectedViewModel, actualGameDefinitionViewModel);
        }

        [Test]
        public void ItSetsTheRegisteredUserEmailAddresses()
        {
            //--arrange
            var expectedDictionary = new Dictionary<int, string>();
            autoMocker.Get<IPlayerRetriever>().Expect(mock =>
                    mock.GetRegisteredUserEmailAddresses(Arg<List<int>>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(expectedDictionary);

            //--act
            autoMocker.ClassUnderTest.Details(_gameDefinitionId, currentUser);

            //--assert
            var args = autoMocker.Get<IPlayerRetriever>().GetArgumentsForCallsMadeOn(mock =>
                mock.GetRegisteredUserEmailAddresses(Arg<List<int>>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
            var playerIds = args.AssertFirstCallIsType<List<int>>();
            playerIds.Count.ShouldBe(2);
            playerIds.ShouldContain(_playerId1);
            playerIds.ShouldContain(_playerId2);

            autoMocker.Get<IGameDefinitionDetailsViewModelBuilder>().AssertWasCalled(mock => mock.Build(
                Arg<GameDefinitionSummary>.Is.Anything, 
                Arg<Dictionary<int, string>>.Is.Same(expectedDictionary),
                Arg<ApplicationUser>.Is.Anything));
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
            const int nonExistentGameDefinitionId = 999999;

            autoMocker.Get<IGameDefinitionRetriever>().BackToRecord(BackToRecordOptions.All);
            autoMocker.Get<IGameDefinitionRetriever>().Replay();
            autoMocker.Get<IGameDefinitionRetriever>().Expect(mock => mock.GetGameDefinitionDetails(
                Arg<int>.Is.Anything,
                Arg<int>.Is.Anything))
                .Throw(new EntityDoesNotExistException<GameDefinition>(nonExistentGameDefinitionId));

            var result = autoMocker.ClassUnderTest.Details(nonExistentGameDefinitionId, currentUser) as HttpStatusCodeResult;

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
