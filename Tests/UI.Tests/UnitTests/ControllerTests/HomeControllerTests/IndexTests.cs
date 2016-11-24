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

using System;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.Facades;
using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayerAchievements;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using UI.Controllers;
using UI.Models.GamingGroup;
using UI.Models.Home;
using UI.Models.Nemeses;
using UI.Models.Players;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Paging;
using UI.Models.GameDefinitionModels;
using UI.Transformations;
using PagedList;
using UI.Mappers;
using UI.Mappers.CustomMappers;
using UI.Mappers.Interfaces;

namespace UI.Tests.UnitTests.ControllerTests.HomeControllerTests
{
    [TestFixture]
    public class IndexTests : HomeControllerTestBase
    {
        private PublicGameSummary expectedPublicGameSummary;
        private List<NemesisChangeViewModel> expectedNemesisChangeViewModels;
        private TopGamingGroupSummary expectedTopGamingGroup;
        private TrendingGame _expectedTrendingGame;
        private TrendingGameViewModel _expectedTrendingGameViewModel;
        private TopGamingGroupSummaryViewModel expectedTopGamingGroupViewModel;
        private ViewResult viewResult;

        List<PlayerAchievement> recentAchievementsUnlocks;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();


            recentAchievementsUnlocks = new List<PlayerAchievement>
            {
                new PlayerAchievement()
                {
                    DateCreated = DateTime.UtcNow,
                    LastUpdatedDate = DateTime.UtcNow,
                    AchievementId = AchievementId.BoardGameGeek2016_10x10
                }


            };

            _autoMocker.Get<IRecentPlayerAchievementsUnlockedRetreiver>().Expect(mock => mock.GetResults(Arg<GetRecentPlayerAchievementsUnlockedQuery>.Is.Anything))
                .Return(recentAchievementsUnlocks.ToPagedList(1, HomeController.NUMBER_OF_RECENT_ACHIEVEMENTS_TO_SHOW));


            expectedPublicGameSummary = new PublicGameSummary();
            var publicGameSummaries = new List<PublicGameSummary>()
            {
                expectedPublicGameSummary
            };
            _autoMocker.Get<IRecentPublicGamesRetriever>().Expect(mock => mock.GetResults(Arg<RecentlyPlayedGamesFilter>.Matches(x => x.NumberOfGamesToRetrieve == HomeController.NUMBER_OF_RECENT_PUBLIC_GAMES_TO_SHOW)))
                .Return(publicGameSummaries);

            var expectedNemesisChanges = new List<NemesisChange>();
            _autoMocker.Get<INemesisHistoryRetriever>().Expect(mock => mock.GetRecentNemesisChanges(HomeController.NUMBER_OF_RECENT_NEMESIS_CHANGES_TO_SHOW))
                                   .Return(expectedNemesisChanges);

            expectedNemesisChangeViewModels = new List<NemesisChangeViewModel>();
            _autoMocker.Get<INemesisChangeViewModelBuilder>().Expect(mock => mock.Build(expectedNemesisChanges))
                                         .Return(expectedNemesisChangeViewModels);

            expectedTopGamingGroup = new TopGamingGroupSummary()
            {
                GamingGroupId = 1,
                GamingGroupName = "gaming group name",
                NumberOfGamesPlayed = 2,
                NumberOfPlayers = 3
            };
            var expectedTopGamingGroupSummaries = new List<TopGamingGroupSummary>()
            {
                expectedTopGamingGroup
            };
            _autoMocker.Get<ITopGamingGroupsRetriever>().Expect(mock => mock.GetResults(HomeController.NUMBER_OF_TOP_GAMING_GROUPS_TO_SHOW))
                                    .Return(expectedTopGamingGroupSummaries);
            _autoMocker.Get<ITransformer>()
                .Expect(mock => mock.Transform<TopGamingGroupSummaryViewModel>(expectedTopGamingGroupSummaries[0]))
                .Return(expectedTopGamingGroupViewModel);

            _expectedTrendingGame = new TrendingGame
            {
                BoardGameGeekGameDefinitionId = 1,
                GamesPlayed = 1,
                GamingGroupsPlayingThisGame = 2,
                ThumbnailImageUrl = "some thumbnail"
            };
            var expectedTopGames = new List<TrendingGame>
            {
                _expectedTrendingGame
            };
            _expectedTrendingGameViewModel = new TrendingGameViewModel();
            var trendingGamesRequest = new TrendingGamesRequest(HomeController.NUMBER_OF_TRENDING_GAMES_TO_SHOW, HomeController.NUMBER_OF_DAYS_OF_TRENDING_GAMES);
            _autoMocker.Get<ITrendingGamesRetriever>().Expect(mock => mock.GetResults(Arg<TrendingGamesRequest>.Is.Equal(trendingGamesRequest))).Return(expectedTopGames);
            _autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<TrendingGameViewModel>(expectedTopGames[0])).Return(_expectedTrendingGameViewModel);

            viewResult = _autoMocker.ClassUnderTest.Index() as ViewResult;

        }

        [Test]
        public void ItReturnsAnIndexView()
        {
            Assert.AreEqual(MVC.Home.Views.Index, viewResult.ViewName);
        }

        [Test]
        public void TheIndexHasTheRecentPlayerAchievementUnlocks()
        {
            var actualViewModel = (HomeIndexViewModel)viewResult.ViewData.Model;

            Assert.AreEqual(recentAchievementsUnlocks.First().AchievementId, actualViewModel.RecentAchievementsUnlocked[0].AchievementId);
        }

        [Test]
        public void TheIndexHasRecentPublicGameSummaries()
        {
            var actualViewModel = (HomeIndexViewModel)viewResult.ViewData.Model;

            Assert.AreSame(expectedPublicGameSummary, actualViewModel.RecentPublicGames[0]);
        }

        [Test]
        public void TheIndexHasTopGames()
        {
            var actualViewModel = (HomeIndexViewModel)viewResult.ViewData.Model;

            Assert.AreSame(_expectedTrendingGameViewModel, actualViewModel.TrendingGames[0]);
        }

        [Test]
        public void TheIndexHasTopGamingGroups()
        {
            var actualViewModel = (HomeIndexViewModel)viewResult.ViewData.Model;

            Assert.AreSame(expectedTopGamingGroupViewModel, actualViewModel.TopGamingGroups[0]);
        }
    }
}
