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

using System.Collections.Generic;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.GamingGroups;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.DataAccess;
using BusinessLogic.Facades;
using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayerAchievements;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Paging;
using PagedList;
using UI.Controllers.Helpers;
using UI.Mappers.Extensions;
using UI.Mappers.Interfaces;
using UI.Models.GameDefinitionModels;
using UI.Models.GamingGroup;
using UI.Models.Home;
using UI.Models.Players;
using UI.Transformations;

namespace UI.Controllers
{
    public partial class HomeController : BaseController
    {
        internal const int NUMBER_OF_RECENT_ACHIEVEMENTS_TO_SHOW = 10;
        internal const int NUMBER_OF_RECENT_PUBLIC_GAMES_TO_SHOW = 5;
        internal const int NUMBER_OF_RECENT_NEMESIS_CHANGES_TO_SHOW = 5;
        internal const int NUMBER_OF_TOP_GAMING_GROUPS_TO_SHOW = 15;
        internal const int NUMBER_OF_DAYS_OF_TRENDING_GAMES = 90;
        internal const int NUMBER_OF_TRENDING_GAMES_TO_SHOW = 5;

        private readonly IRecentPublicGamesRetriever _recentPublicGamesRetriever;
        private readonly ITopGamingGroupsRetriever _topGamingGroupsRetriever;
        private readonly ITrendingGamesRetriever _trendingGamesRetriever;
        private readonly ITransformer _transformer;
        private readonly IRecentPlayerAchievementsUnlockedRetreiver _recentPlayerAchievementsUnlockedRetreiver;
        private readonly IMapperFactory _mapperFactory;

        public HomeController(
            IRecentPublicGamesRetriever recentPublicGamesRetriever,
            ITopGamingGroupsRetriever topGamingGroupsRetriever,
            ITrendingGamesRetriever trendingGamesRetriever,
            ITransformer transformer,            
            IRecentPlayerAchievementsUnlockedRetreiver recentPlayerAchievementsUnlockedRetreiver,
            IMapperFactory mapperFactory, IDataContext dataContext)
        {
            _recentPublicGamesRetriever = recentPublicGamesRetriever;
            _topGamingGroupsRetriever = topGamingGroupsRetriever;
            _trendingGamesRetriever = trendingGamesRetriever;
            _transformer = transformer;
            _recentPlayerAchievementsUnlockedRetreiver = recentPlayerAchievementsUnlockedRetreiver;
            _mapperFactory = mapperFactory;
        }

        public virtual ActionResult Index()
        {
            var recentPlayerAchievementWinners = _recentPlayerAchievementsUnlockedRetreiver.GetResults(new GetRecentPlayerAchievementsUnlockedQuery {PageSize = NUMBER_OF_RECENT_ACHIEVEMENTS_TO_SHOW });
            var recentPlayerAchievementWinnerViewModel = recentPlayerAchievementWinners.ToTransformedPagedList<PlayerAchievementWinner, PlayerAchievementWinnerViewModel>(_transformer);

            var recentlyPlayedGamesFilter = new RecentlyPlayedGamesFilter
            {
                NumberOfGamesToRetrieve = NUMBER_OF_RECENT_PUBLIC_GAMES_TO_SHOW
            };
            var publicGameSummaries = _recentPublicGamesRetriever.GetResults(recentlyPlayedGamesFilter);

            var topGamingGroups = _topGamingGroupsRetriever.GetResults(NUMBER_OF_TOP_GAMING_GROUPS_TO_SHOW);

            var topGamingGroupViewModels = topGamingGroups.Select(_transformer.Transform<TopGamingGroupSummaryViewModel>).ToList();

            var trendingGamesRequest = new TrendingGamesRequest(NUMBER_OF_TRENDING_GAMES_TO_SHOW, NUMBER_OF_DAYS_OF_TRENDING_GAMES);
            var trendingGames = _trendingGamesRetriever.GetResults(trendingGamesRequest);
            var trendingGameViewModels = trendingGames.Select(_transformer.Transform<TrendingGameViewModel>).ToList();

            var homeIndexViewModel = new HomeIndexViewModel()
            {
                RecentAchievementsUnlocked = recentPlayerAchievementWinnerViewModel,
                RecentPublicGames = publicGameSummaries,
                TopGamingGroups = topGamingGroupViewModels,
                TrendingGames = trendingGameViewModels
            };
            ViewBag.NumTrendingGameDays = NUMBER_OF_DAYS_OF_TRENDING_GAMES;
            return View(MVC.Home.Views.Index, homeIndexViewModel);
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