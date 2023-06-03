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
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NemeStats.TestingHelpers.NemeStatsTestingExtensions;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using UI.Controllers;
using UI.Controllers.Helpers;
using UI.Models.PlayedGame;
using UI.Models.Players;
using UI.Transformations.PlayerTransformations;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class DetailsTests : PlayerControllerTestBase
    {
        private readonly int _playerId = 1;
        private string _expectedMinionUrl;
        private readonly Dictionary<int, string> _expectedPlayerIdToRegisteredUserEmailDictionary = new Dictionary<int, string>();

        public override void SetUp()
        {
            base.SetUp();
            
            _expectedMinionUrl = autoMocker.ClassUnderTest.Url.Action(MVC.Player.ActionNames.Details, MVC.Player.Name, new { id = _playerId }, "HTTPS") + "#minions";

            autoMocker.Get<IPlayerRetriever>().Expect(mock =>
                    mock.GetRegisteredUserEmailAddresses(Arg<IList<int>>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(_expectedPlayerIdToRegisteredUserEmailDictionary);
        }

        private void SetupMinimumExpectations()
        {
            autoMocker.Get<IPlayerRetriever>().Expect(mock => mock.GetPlayerDetails(0, 0))
                .IgnoreArguments()
                .Return(new PlayerDetails
                {
                    PlayerGameResults = new List<PlayerGameResult>()
                });
            autoMocker.Get<IPlayerDetailsViewModelBuilder>().Expect(mock => mock.Build(null, null, null))
                .IgnoreArguments()
                .Return(new PlayerDetailsViewModel());
        }

        [Test]
        public void ItRetrievesRequestedPlayer()
        {
            SetupMinimumExpectations();

            autoMocker.ClassUnderTest.Details(_playerId, currentUser);

            autoMocker.Get<IPlayerRetriever>().AssertWasCalled(x => x.GetPlayerDetails(_playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE));
        }

        [Test]
        public void ItReturnsThePlayerDetailsViewWhenThePlayerIsFound()
        {
            PlayerDetails playerDetails = new PlayerDetails(){ PlayerGameResults = new List<PlayerGameResult>() };
            autoMocker.Get<IPlayerRetriever>().Expect(playerLogic => playerLogic.GetPlayerDetails(_playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE))
                .Repeat.Once()
                .Return(playerDetails);

            ViewResult viewResult = autoMocker.ClassUnderTest.Details(_playerId, currentUser) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Details, viewResult.ViewName);
        }

        [Test]
        public void ItSetsThePlayerDetailsViewModelForTheFoundPlayer()
        {
            //--arrange
            PlayerDetails playerDetails = new PlayerDetails() { Id = _playerId, PlayerGameResults = new List<PlayerGameResult>() };
            autoMocker.Get<IPlayerRetriever>().Expect(playerLogic => playerLogic.GetPlayerDetails(_playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE))
                .Repeat.Once()
                .Return(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel()
            {
                PlayerId = _playerId,
                PlayerGameResultDetails = new List<GameResultViewModel>()
            };
            autoMocker.Get<IPlayerDetailsViewModelBuilder>().Expect(viewModelBuilder => viewModelBuilder.Build(playerDetails, _expectedPlayerIdToRegisteredUserEmailDictionary, _expectedMinionUrl, currentUser))
                .Repeat
                .Once()
                .Return(playerDetailsViewModel);

            //--act
            ViewResult viewResult = autoMocker.ClassUnderTest.Details(_playerId, currentUser) as ViewResult;

            //--assert
            Assert.AreEqual(playerDetailsViewModel, viewResult.Model);
        }

        [Test]
        public void It_Sets_The_Registered_Users_Email_Addresses_Including_The_Main_Player()
        {
            //--arrange
            var expectedPlayerId1 = 50;
            var expectedPlayerId2 = 51;
            autoMocker.Get<IPlayerRetriever>().Expect(mock => mock.GetPlayerDetails(0, 0))
                .IgnoreArguments()
                .Return(new PlayerDetails
                {
                    PlayerGameResults = new List<PlayerGameResult>(),
                    PlayerVersusPlayersStatistics = new List<PlayerVersusPlayerStatistics>
                    {
                        new PlayerVersusPlayerStatistics
                        {
                            OpposingPlayerId = expectedPlayerId1
                        },
                        new PlayerVersusPlayerStatistics
                        {
                            OpposingPlayerId = expectedPlayerId2
                        }
                    }
                });
            autoMocker.Get<IPlayerDetailsViewModelBuilder>().Expect(mock => mock.Build(null, null, null))
                .IgnoreArguments()
                .Return(new PlayerDetailsViewModel());

            var expectedDictionary = new Dictionary<int, string>();
            autoMocker.Get<IPlayerRetriever>()
                .Expect(mock =>
                    mock.GetRegisteredUserEmailAddresses(Arg<IList<int>>.Is.Anything,
                        Arg<ApplicationUser>.Is.Anything))
                .Return(expectedDictionary);

            //--act
            autoMocker.ClassUnderTest.Details(_playerId, currentUser);

            //--assert
            var args = autoMocker.Get<IPlayerRetriever>()
                .GetArgumentsForCallsMadeOn(mock =>
                    mock.GetRegisteredUserEmailAddresses(Arg<IList<int>>.Is.Anything,
                        Arg<ApplicationUser>.Is.Anything));
            var playerIds = args.AssertFirstCallIsType<IList<int>>();
            playerIds.Count.ShouldBe(3);
            playerIds.ShouldContain(expectedPlayerId1);
            playerIds.ShouldContain(expectedPlayerId2);
            playerIds.ShouldContain(_playerId);

            var applicationUser = args.AssertCallIsType<ApplicationUser>(0, 1);
            applicationUser.ShouldBeSameAs(currentUser);

            //--the fact that .Build is called with this dictionary as a parameter is implicitly tested elsewhere. This isn't great, but not worth refactoring for now
        }

        [Test]
        public void ItOnlyRetrievesTheSpecifiedNumberOfPlayers()
        {
            SetupMinimumExpectations();

            autoMocker.ClassUnderTest.Details(_playerId, currentUser);

            autoMocker.Get<IPlayerRetriever>().AssertWasCalled(mock => mock.GetPlayerDetails(_playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE));
        }

        [Test]
        public void ItPutsTheRecentGamesMessageOnTheViewBag()
        {
            PlayerDetails playerDetails = new PlayerDetails(){ PlayerGameResults = new List<PlayerGameResult>() };
            autoMocker.Get<IPlayerRetriever>().Expect(playerLogic => playerLogic.GetPlayerDetails(
                _playerId, 
                PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE))
                .Repeat.Once()
                .Return(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel()
            {
                PlayerId = _playerId,
                PlayerGameResultDetails = new List<GameResultViewModel>()
            };
            autoMocker.Get<IPlayerDetailsViewModelBuilder>().Expect(viewModelBuilder => viewModelBuilder.Build(playerDetails, _expectedPlayerIdToRegisteredUserEmailDictionary, _expectedMinionUrl, currentUser))
                .Repeat
                .Once()
                .Return(playerDetailsViewModel);
            string expectedMessage = "expected message";
            autoMocker.Get<IShowingXResultsMessageBuilder>().Expect(mock => mock.BuildMessage(
                PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE,
                playerDetailsViewModel.PlayerGameResultDetails.Count))
                    .Return(expectedMessage);

            autoMocker.ClassUnderTest.Details(_playerId, currentUser);

            Assert.AreEqual(expectedMessage, autoMocker.ClassUnderTest.ViewBag.RecentGamesMessage);
        }

        [Test]
        public void ItReturnsAnHttpNotFoundStatusCodeWhenPlayerDoesNotExist()
        {
            const int nonExistentPlayerId = -1;
            autoMocker.Get<IPlayerRetriever>().BackToRecord(BackToRecordOptions.All);
            autoMocker.Get<IPlayerRetriever>().Expect(mock => 
                mock.GetPlayerDetails(nonExistentPlayerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE))
                .Throw(new EntityDoesNotExistException<Player>(nonExistentPlayerId));
            autoMocker.Get<IPlayerRetriever>().Replay();

            var result = autoMocker.ClassUnderTest.Details(nonExistentPlayerId, currentUser) as HttpStatusCodeResult;

            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }
    }
}