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
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UI.Controllers;
using UI.Models.GamingGroup;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class IndexTests : GamingGroupControllerTestBase
    {
        private GamingGroupSummary gamingGroupSummary;
        private GamingGroupViewModel gamingGroupViewModel;

        [Test]
        public override void SetUp()
        {
            base.SetUp();

            gamingGroupSummary = new GamingGroupSummary()
            {
                PlayedGames = new List<PlayedGame>()
            };
            gamingGroupViewModel = new GamingGroupViewModel();

            gamingGroupRetrieverMock.Expect(mock => mock.GetGamingGroupDetails(
                currentUser.CurrentGamingGroupId.Value,
                GamingGroupController.MAX_NUMBER_OF_RECENT_GAMES))
                .Repeat.Once()
                .Return(gamingGroupSummary);

            gamingGroupViewModelBuilderMock.Expect(mock => mock.Build(gamingGroupSummary, currentUser))
                .Return(gamingGroupViewModel);
        }

        [Test]
        public void ItReturnsTheIndexView()
        {
            ViewResult viewResult = gamingGroupControllerPartialMock.Index(currentUser) as ViewResult;

            Assert.AreEqual(MVC.GamingGroup.Views.Index, viewResult.ViewName);
        }

        [Test]
        public void ItAddsAGamingGroupViewModelToTheView()
        {
            ViewResult viewResult = gamingGroupControllerPartialMock.Index(currentUser) as ViewResult;

            Assert.AreSame(gamingGroupViewModel, viewResult.Model);
        }

        [Test]
        public void ItAddsTheRecentlyPlayedGamesMessageToTheViewBag()
        {
            string expectedMessage = "expected message";
            showingXResultsMessageBuilderMock.Expect(mock => mock.BuildMessage(
                 GamingGroupController.MAX_NUMBER_OF_RECENT_GAMES,
                 gamingGroupSummary.PlayedGames.Count))
                     .Return(expectedMessage);

            gamingGroupControllerPartialMock.Index(currentUser);

            Assert.AreEqual("Recent Games " + expectedMessage, gamingGroupControllerPartialMock.ViewBag.PlayedGamesPartialPanelTitle);
        }
    }
}
