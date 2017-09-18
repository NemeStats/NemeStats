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

using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Facades;
using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayerAchievements;
using BusinessLogic.Models.Achievements;
using UI.Controllers;
using UI.Models.GamingGroup;
using UI.Models.Home;
using UI.Models.Nemeses;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Paging;
using UI.Models.GameDefinitionModels;
using UI.Transformations;
using PagedList;
using Shouldly;
using UI.Models.Players;

namespace UI.Tests.UnitTests.ControllerTests.HomeControllerTests
{
    [TestFixture]
    public class IndexTests : HomeControllerTestBase
    {
        private List<NemesisChangeViewModel> _expectedNemesisChangeViewModels;
        private TopGamingGroupSummary _expectedTopGamingGroup;
        private TrendingGame _expectedTrendingGame;
        private TrendingGameViewModel _expectedTrendingGameViewModel;
        private TopGamingGroupSummaryViewModel _expectedTopGamingGroupViewModel;
        private ViewResult _viewResult;

        private IPagedList<PlayerAchievementWinner> _recentAchievementsUnlocks;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _expectedTopGamingGroup = new TopGamingGroupSummary()
            {
                GamingGroupId = 1,
                GamingGroupName = "gaming group name",
                NumberOfGamesPlayed = 2,
                NumberOfPlayers = 3
            };
            var expectedTopGamingGroupSummaries = new List<TopGamingGroupSummary>()
            {
                _expectedTopGamingGroup
            };
            _autoMocker.Get<ITopGamingGroupsRetriever>().Expect(mock => mock.GetResults(HomeController.NUMBER_OF_TOP_GAMING_GROUPS_TO_SHOW))
                                    .Return(expectedTopGamingGroupSummaries);

            _expectedTopGamingGroupViewModel = new TopGamingGroupSummaryViewModel();
            _autoMocker.Get<ITransformer>()
                .Expect(mock => mock.Transform<TopGamingGroupSummaryViewModel>(expectedTopGamingGroupSummaries[0]))
                .Return(_expectedTopGamingGroupViewModel);

            _viewResult = _autoMocker.ClassUnderTest.Index() as ViewResult;
        }

        [Test]
        public void ItReturnsAnIndexView()
        {
            _viewResult.ViewName.ShouldBe(MVC.Home.Views.Index);
        }

        [Test]
        public void TheIndexHasTopGamingGroups()
        {
            var actualViewModel = (HomeIndexViewModel)_viewResult.ViewData.Model;

            Assert.AreSame(_expectedTopGamingGroupViewModel, actualViewModel.TopGamingGroups[0]);
        }
    }
}
