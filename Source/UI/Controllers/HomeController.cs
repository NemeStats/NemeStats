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
using BusinessLogic.Paging;
using UI.Controllers.Helpers;
using UI.Mappers.Extensions;
using UI.Mappers.Interfaces;
using UI.Models.GameDefinitionModels;
using UI.Models.GamingGroup;
using UI.Models.Home;
using UI.Models.Players;

namespace UI.Controllers
{
    public partial class HomeController : BaseController
    {
        public const int NUMBER_OF_RECENT_ACHIEVEMENTS_TO_SHOW = 10;
        public const int NUMBER_OF_RECENT_PUBLIC_GAMES_TO_SHOW = 5;
        public const int NUMBER_OF_TOP_GAMING_GROUPS_TO_SHOW = 15;
        public const int NUMBER_OF_DAYS_OF_TRENDING_GAMES = 90;
        public const int NUMBER_OF_TRENDING_GAMES_TO_SHOW = 5;

        private readonly IRecentPublicGamesRetriever _recentPublicGamesRetriever;
        private readonly ITopGamingGroupsRetriever _topGamingGroupsRetriever;
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
            _topGamingGroupsRetriever = topGamingGroupsRetriever;
            _trendingGamesRetriever = trendingGamesRetriever;
            _transformer = transformer;
            _recentPlayerAchievementsUnlockedRetriever = recentPlayerAchievementsUnlockedRetriever;
        }

        [HttpGet]
        public virtual ActionResult Index()
        {

            var homeIndexViewModel = new HomeIndexViewModel
            {
            };
            return View(MVC.Home.Views.Index, homeIndexViewModel);
        }

        [HttpGet]
        public virtual ActionResult TrendingGames()
        {
            var trendingGamesRequest = new TrendingGamesRequest(NUMBER_OF_TRENDING_GAMES_TO_SHOW, NUMBER_OF_DAYS_OF_TRENDING_GAMES);
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
                MaxDate = DateTime.UtcNow.Date.AddDays(1)
            };
            var publicGameSummaries = _recentPublicGamesRetriever.GetResults(recentlyPlayedGamesFilter);
            return PartialView(MVC.PlayedGame.Views._RecentlyPlayedGamesPartial, publicGameSummaries);
        }

        [HttpGet]
        public virtual ActionResult RecentAchievementsUnlocked()
        {
            var recentPlayerAchievementWinners = _recentPlayerAchievementsUnlockedRetriever.GetResults(new GetRecentPlayerAchievementsUnlockedQuery { PageSize = NUMBER_OF_RECENT_ACHIEVEMENTS_TO_SHOW });
            var recentPlayerAchievementWinnerViewModel = recentPlayerAchievementWinners.ToTransformedPagedList<PlayerAchievementWinner, PlayerAchievementWinnerViewModel>(_transformer);

            return PartialView(MVC.Achievement.Views._RecentAchievementsUnlocked, recentPlayerAchievementWinnerViewModel);
        }

        [HttpGet]
        public virtual ActionResult TopGamingGroups()
        {
            var topGamingGroups = _topGamingGroupsRetriever.GetResults(NUMBER_OF_TOP_GAMING_GROUPS_TO_SHOW);

            var topGamingGroupViewModels = topGamingGroups.Select(_transformer.Transform<TopGamingGroupSummaryViewModel>).ToList();
            return PartialView(MVC.GamingGroup.Views._TopGamingGroupsPartial, topGamingGroupViewModels);
        }

        public virtual ActionResult About()
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
    }
}