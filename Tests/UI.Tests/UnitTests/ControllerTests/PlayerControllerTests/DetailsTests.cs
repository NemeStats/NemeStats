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
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using Rhino.Mocks;
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
        private int playerId = 1;
        private string expectedMinionUrl;

        public override void SetUp()
        {
            base.SetUp();
            
            expectedMinionUrl = autoMocker.ClassUnderTest.Url.Action(MVC.Player.ActionNames.Details, MVC.Player.Name, new { id = playerId }, "HTTPS") + "#minions";
        }

        [Test]
        public void ItNeverReturnsNull()
        {
            Assert.NotNull(autoMocker.ClassUnderTest.Details(null, null));
        }

        [Test]
        public void ItReturnsBadHttpStatusWhenNoPlayerIdGiven()
        {
            HttpStatusCodeResult actualResult = autoMocker.ClassUnderTest.Details(null, null) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, actualResult.StatusCode);
        }

        [Test]
        public void ItReturns404StatusWhenNoPlayerIsFound()
        {
            HttpStatusCodeResult actualResult = autoMocker.ClassUnderTest.Details(-1, null) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, actualResult.StatusCode);
        }

        [Test]
        public void ItRetrievesRequestedPlayer()
        {
            autoMocker.ClassUnderTest.Details(playerId, currentUser);

            autoMocker.Get<IPlayerRetriever>().AssertWasCalled(x => x.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE));
        }

        [Test]
        public void ItReturnsThePlayerDetailsViewWhenThePlayerIsFound()
        {
            PlayerDetails playerDetails = new PlayerDetails(){ PlayerGameResults = new List<PlayerGameResult>() };
            autoMocker.Get<IPlayerRetriever>().Expect(playerLogic => playerLogic.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE))
                .Repeat.Once()
                .Return(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel()
            {
                PlayerId = playerId,
                PlayerGameResultDetails = new List<GameResultViewModel>()
            };
            autoMocker.Get<IPlayerDetailsViewModelBuilder>().Expect(viewModelBuilder => viewModelBuilder.Build(playerDetails, expectedMinionUrl, currentUser))
                .Repeat
                .Once()
                .Return(playerDetailsViewModel);
            ViewResult viewResult = autoMocker.ClassUnderTest.Details(playerId, currentUser) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Details, viewResult.ViewName);
        }

        [Test]
        public void ItSetsThePlayerDetailsViewModelForTheFoundPlayer()
        {
            PlayerDetails playerDetails = new PlayerDetails() { Id = playerId, PlayerGameResults = new List<PlayerGameResult>() };
            autoMocker.Get<IPlayerRetriever>().Expect(playerLogic => playerLogic.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE))
                .Repeat.Once()
                .Return(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel()
            {
                PlayerId = playerId,
                PlayerGameResultDetails = new List<GameResultViewModel>()
            };
            autoMocker.Get<IPlayerDetailsViewModelBuilder>().Expect(viewModelBuilder => viewModelBuilder.Build(playerDetails, expectedMinionUrl, currentUser))
                .Repeat
                .Once()
                .Return(playerDetailsViewModel);

            ViewResult viewResult = autoMocker.ClassUnderTest.Details(playerId, currentUser) as ViewResult;

            Assert.AreEqual(playerDetailsViewModel, viewResult.Model);
        }

        [Test]
        public void ItOnlyRetrievesTheSpecifiedNumberOfPlayers()
        {
            autoMocker.ClassUnderTest.Details(playerId, currentUser);

            autoMocker.Get<IPlayerRetriever>().AssertWasCalled(mock => mock.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE));
        }

        [Test]
        public void ItPutsTheRecentGamesMessageOnTheViewbag()
        {
            PlayerDetails playerDetails = new PlayerDetails(){ PlayerGameResults = new List<PlayerGameResult>() };
            autoMocker.Get<IPlayerRetriever>().Expect(playerLogic => playerLogic.GetPlayerDetails(
                playerId, 
                PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE))
                .Repeat.Once()
                .Return(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel()
            {
                PlayerId = playerId,
                PlayerGameResultDetails = new List<GameResultViewModel>()
            };
            autoMocker.Get<IPlayerDetailsViewModelBuilder>().Expect(viewModelBuilder => viewModelBuilder.Build(playerDetails, expectedMinionUrl, currentUser))
                .Repeat
                .Once()
                .Return(playerDetailsViewModel);
            string expectedMessage = "expected message";
            autoMocker.Get<IShowingXResultsMessageBuilder>().Expect(mock => mock.BuildMessage(
                PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE,
                playerDetailsViewModel.PlayerGameResultDetails.Count))
                    .Return(expectedMessage);

            autoMocker.ClassUnderTest.Details(playerId, currentUser);

            Assert.AreEqual(expectedMessage, autoMocker.ClassUnderTest.ViewBag.RecentGamesMessage);
        }
    }
}