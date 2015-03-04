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
using NUnit.Framework;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Rhino.Mocks;
using BusinessLogic.Models;
using UI.Models.PlayedGame;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    [TestFixture]
    public class DetailsTests : PlayedGameControllerTestBase
    {
        [Test]
        public void ItNeverReturnsNull()
        {
            Assert.NotNull(playedGameController.Details(null, null));
        }

        [Test]
        public void ItReturnsBadHttpStatusWhenNoPlayedGameIdGiven()
        {
            HttpStatusCodeResult actualResult = playedGameController.Details(null, null) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, actualResult.StatusCode);
        }

        [Test]
        public void ItReturns404StatusWhenNoPlayedGameIsFound()
        {
            HttpStatusCodeResult actualResult = playedGameController.Details(-1, null) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, actualResult.StatusCode);
        }
        
        [Test]
        public void ItRetrievesRequestedPlayedGame()
        {
            int playedGameId = 1351;
            playedGameController.Details(playedGameId, currentUser);

            playedGameRetriever.AssertWasCalled(x => x.GetPlayedGameDetails(playedGameId));
        }
        
        [Test]
        public void ItReturnsThePlayedGameDetailsViewWhenThePlayedGameIsFound()
        {
            int playedGameId = 1351;
            playedGameRetriever.Expect(playedGameLogic => playedGameLogic.GetPlayedGameDetails(playedGameId))
                .Repeat.Once()
                .Return(new PlayedGame());
            ViewResult playedGameDetails = playedGameController.Details(playedGameId, currentUser) as ViewResult;

            Assert.AreEqual(MVC.PlayedGame.Views.Details, playedGameDetails.ViewName);
        }

        [Test]
        public void ItReturnsThePlayedGameDetailsViewForTheFoundPlayedGame()
        {
            int playedGameId = 13541;

            PlayedGame playedGame = new PlayedGame() { Id = 123 };
            playedGameRetriever.Expect(x => x.GetPlayedGameDetails(playedGameId))
                .Repeat.Once()
                .Return(playedGame);
            PlayedGameDetailsViewModel playedGameDetails = new PlayedGameDetailsViewModel();
            playedGameDetailsBuilderMock.Expect(builder => builder.Build(playedGame, currentUser)).Repeat.Once()
                .Return(playedGameDetails);

            ViewResult result = playedGameController.Details(playedGameId, currentUser) as ViewResult;

            PlayedGameDetailsViewModel viewModel = (PlayedGameDetailsViewModel)result.ViewData.Model;
            Assert.AreEqual(playedGameDetails, viewModel);
        } 
    }
}
