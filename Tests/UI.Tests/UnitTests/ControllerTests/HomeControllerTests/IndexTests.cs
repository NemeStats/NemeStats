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
using AutoMapper;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UI.Controllers;
using UI.Models.GamingGroup;
using UI.Models.Home;
using UI.Models.Nemeses;
using UI.Models.Players;
using BusinessLogic.Models.GamingGroups;

namespace UI.Tests.UnitTests.ControllerTests.HomeControllerTests
{
    [TestFixture]
    public class IndexTests : HomeControllerTestBase
    {
        private TopPlayerViewModel expectedPlayer;
        private PublicGameSummary expectedPublicGameSummary;
        private List<NemesisChangeViewModel> expectedNemesisChangeViewModels;
        private TopGamingGroupSummary expectedTopGamingGroup;
        private TopGamingGroupSummaryViewModel expectedTopGamingGroupViewModel;
        private ViewResult viewResult;
            
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            List<TopPlayer> topPlayers = new List<TopPlayer>()
            {
                new TopPlayer()
            };

            playerSummaryBuilderMock.Expect(mock => mock.GetTopPlayers(HomeController.NUMBER_OF_TOP_PLAYERS_TO_SHOW))
                .Return(topPlayers);
            expectedPlayer = new TopPlayerViewModel();
            topPlayerViewModelBuilderMock.Expect(mock => mock.Build(Arg<TopPlayer>.Is.Anything))
                .Return(expectedPlayer);

            expectedPublicGameSummary = new PublicGameSummary();
            List<PublicGameSummary> publicGameSummaries = new List<PublicGameSummary>()
            {
                expectedPublicGameSummary
            };
            playedGameRetrieverMock.Expect(mock => mock.GetRecentPublicGames(HomeController.NUMBER_OF_RECENT_PUBLIC_GAMES_TO_SHOW))
                .Return(publicGameSummaries);

            List<NemesisChange> expectedNemesisChanges = new List<NemesisChange>();
            nemesisHistoryRetrieverMock.Expect(mock => mock.GetRecentNemesisChanges(HomeController.NUMBER_OF_RECENT_NEMESIS_CHANGES_TO_SHOW))
                                   .Return(expectedNemesisChanges);

            expectedNemesisChangeViewModels = new List<NemesisChangeViewModel>();
            nemesisChangeViewModelBuilderMock.Expect(mock => mock.Build(expectedNemesisChanges))
                                         .Return(expectedNemesisChangeViewModels);

            expectedTopGamingGroup = new TopGamingGroupSummary()
            {
                GamingGroupId = 1,
                GamingGroupName = "gaming group name",
                NumberOfGamesPlayed = 2,
                NumberOfPlayers = 3
            };
            List<TopGamingGroupSummary> expectedTopGamingGroupSummaries = new List<TopGamingGroupSummary>()
            {
                expectedTopGamingGroup
            };
            gamingGroupRetrieverMock.Expect(mock => mock.GetTopGamingGroups(HomeController.NUMBER_OF_TOP_GAMING_GROUPS_TO_SHOW))
                                    .Return(expectedTopGamingGroupSummaries);
            expectedTopGamingGroupViewModel = Mapper.Map<TopGamingGroupSummary, TopGamingGroupSummaryViewModel>(expectedTopGamingGroupSummaries[0]);

            HomeIndexViewModel indexViewModel = new HomeIndexViewModel();
            viewResult = homeControllerPartialMock.Index() as ViewResult;
        }

        [Test]
        public void ItReturnsAnIndexView()
        {
            Assert.AreEqual(MVC.Home.Views.Index, viewResult.ViewName);
        }

        [Test]
        public void TheIndexHasTheRecentPlayerSummaries()
        {
            HomeIndexViewModel actualViewModel = (HomeIndexViewModel)viewResult.ViewData.Model;

            Assert.AreSame(expectedPlayer, actualViewModel.TopPlayers[0]);
        }

        [Test]
        public void TheIndexHasRecentPublicGameSummaries()
        {
            HomeIndexViewModel actualViewModel = (HomeIndexViewModel)viewResult.ViewData.Model;

            Assert.AreSame(expectedPublicGameSummary, actualViewModel.RecentPublicGames[0]);
        }

        [Test]
        public void TheIndexHasRecentNemesisChanges()
        {
            HomeIndexViewModel actualViewModel = (HomeIndexViewModel)viewResult.ViewData.Model;

            Assert.AreSame(expectedNemesisChangeViewModels, actualViewModel.RecentNemesisChanges);
        }

        [Test]
        public void TheIndexHasTopGamingGroups()
        {
            HomeIndexViewModel actualViewModel = (HomeIndexViewModel)viewResult.ViewData.Model;

            Assert.AreEqual(expectedTopGamingGroupViewModel.GamingGroupId, actualViewModel.TopGamingGroups[0].GamingGroupId);
            Assert.AreEqual(expectedTopGamingGroupViewModel.GamingGroupName, actualViewModel.TopGamingGroups[0].GamingGroupName);
            Assert.AreEqual(expectedTopGamingGroupViewModel.NumberOfGamesPlayed, actualViewModel.TopGamingGroups[0].NumberOfGamesPlayed);
            Assert.AreEqual(expectedTopGamingGroupViewModel.NumberOfPlayers, actualViewModel.TopGamingGroups[0].NumberOfPlayers);

        }
    }
}
