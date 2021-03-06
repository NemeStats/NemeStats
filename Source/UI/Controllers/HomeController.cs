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

#endregion LICENSE

using System;
using BusinessLogic.Models.Games;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.DataAccess;
using BusinessLogic.Facades;
using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayerAchievements;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.User;
using BusinessLogic.Paging;
using UI.Attributes.Filters;
using UI.Controllers.Helpers;
using UI.Mappers.Extensions;
using UI.Mappers.Interfaces;
using UI.Models.GameDefinitionModels;
using UI.Models.Home;
using UI.Models.Players;

namespace UI.Controllers
{
    public partial class HomeController : BaseController
    {
        [Obsolete("This method will go away since users voted down this feature a lot")]
        public const int NUMBER_OF_RECENT_ACHIEVEMENTS_TO_SHOW = 10;
        public const int NUMBER_OF_RECENT_PUBLIC_GAMES_TO_SHOW = 5;
        public const int NUMBER_OF_TOP_GAMING_GROUPS_TO_SHOW = 15;
        public const int NUMBER_OF_DAYS_OF_TRENDING_GAMES = 90;
        public const int A_LOT_OF_DAYS = 10000;
        public const int NUMBER_OF_TRENDING_GAMES_TO_SHOW = 5;
        public const int NUMBER_OF_TOP_GAMES_TO_SHOW = 5;

        private readonly IRecentPublicGamesRetriever _recentPublicGamesRetriever;
        private readonly ITrendingGamesRetriever _trendingGamesRetriever;
        private readonly ITransformer _transformer;
        private readonly IRecentPlayerAchievementsUnlockedRetriever _recentPlayerAchievementsUnlockedRetriever;

        public HomeController(
            IRecentPublicGamesRetriever recentPublicGamesRetriever,
            ITopGamingGroupsRetriever topGamingGroupsRetriever,
            ITrendingGamesRetriever trendingGamesRetriever,
            ITransformer transformer,            
            IRecentPlayerAchievementsUnlockedRetriever recentPlayerAchievementsUnlockedRetriever,
            IMapperFactory mapperFactory, IDataContext dataContext)
        {
            _recentPublicGamesRetriever = recentPublicGamesRetriever;
            _trendingGamesRetriever = trendingGamesRetriever;
            _transformer = transformer;
            _recentPlayerAchievementsUnlockedRetriever = recentPlayerAchievementsUnlockedRetriever;
        }

        [HttpGet]
        [UserContext(RequiresGamingGroup = false)]
        public virtual ActionResult Index(ApplicationUser currentUser)
        {
            var homeIndexViewModel = new HomeIndexViewModel
            {
                ShowQuickStats = currentUser.CurrentGamingGroupId.HasValue,
                ShowLoginPartial = currentUser.IsAnonymousUser()
            };
            return View(MVC.Home.Views.Index, homeIndexViewModel);
        }

        [HttpGet]
        public virtual ActionResult TrendingGames()
        {
            return GetTopGamesPartialView(NUMBER_OF_TRENDING_GAMES_TO_SHOW, NUMBER_OF_DAYS_OF_TRENDING_GAMES);
        }

        [NonAction]
        public virtual PartialViewResult GetTopGamesPartialView(int numberOfGamesToShow, int numberOfDaysToConsider)
        {
            var trendingGamesRequest = new TrendingGamesRequest(numberOfGamesToShow, numberOfDaysToConsider);
            var trendingGames = _trendingGamesRetriever.GetResults(trendingGamesRequest);
            var trendingGameViewModels = trendingGames.Select(_transformer.Transform<TrendingGameViewModel>).ToList();

            ViewBag.NumTrendingGameDays = NUMBER_OF_DAYS_OF_TRENDING_GAMES;
            return PartialView(MVC.GameDefinition.Views._TrendingGamesPartial, trendingGameViewModels);
        }

        [HttpGet]
        public virtual ActionResult RecentPlayedGames()
        {
            var recentlyPlayedGamesFilter = new RecentlyPlayedGamesFilter
            {
                NumberOfGamesToRetrieve = NUMBER_OF_RECENT_PUBLIC_GAMES_TO_SHOW,
                MinDate = DateTime.UtcNow.Date.AddDays(-2),
                MaxDate = DateTime.UtcNow.Date.AddDays(1)
            };
            var publicGameSummaries = _recentPublicGamesRetriever.GetResults(recentlyPlayedGamesFilter);
            return PartialView(MVC.PlayedGame.Views._RecentlyPlayedGamesPartial, publicGameSummaries);
        }

        [Obsolete("Took this widget off of the home page due to too many downvotes")]
        [HttpGet]
        public virtual ActionResult RecentAchievementsUnlocked()
        {
            var recentPlayerAchievementWinners = _recentPlayerAchievementsUnlockedRetriever.GetResults(new GetRecentPlayerAchievementsUnlockedQuery { PageSize = NUMBER_OF_RECENT_ACHIEVEMENTS_TO_SHOW });
            var recentPlayerAchievementWinnerViewModel = recentPlayerAchievementWinners.ToTransformedPagedList<PlayerAchievementWinner, PlayerAchievementWinnerViewModel>(_transformer);

            return PartialView(MVC.Achievement.Views._RecentAchievementsUnlocked, recentPlayerAchievementWinnerViewModel);
        }

        [HttpGet]
        public virtual ActionResult TopGamesEver()
        {
            return GetTopGamesPartialView(NUMBER_OF_TOP_GAMES_TO_SHOW, A_LOT_OF_DAYS);
        }

        public virtual ActionResult About()
        {            
            return View();
        }

        public virtual ActionResult NemeStatsAndroidApp()
        {
            return View();
        }

        public virtual ActionResult AboutNemePoints()
        {
            return View();
        }

        public virtual ActionResult AboutBadgesAndAchievements()
        {
            return View();
        }

        public virtual ActionResult PrivacyNotice()
        {
            return View();
        }
    }
}