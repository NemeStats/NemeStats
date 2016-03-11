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

using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.GamingGroups;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.Logic.BoardGameGeek;
using UI.Controllers.Helpers;
using UI.Models.GameDefinitionModels;
using UI.Models.GamingGroup;
using UI.Models.Home;
using UI.Transformations;
using UI.Transformations.PlayerTransformations;

namespace UI.Controllers
{
    public partial class HomeController : BaseController
    {
        internal const int NUMBER_OF_TOP_PLAYERS_TO_SHOW = 15;
        internal const int NUMBER_OF_RECENT_PUBLIC_GAMES_TO_SHOW = 5;
        internal const int NUMBER_OF_RECENT_NEMESIS_CHANGES_TO_SHOW = 5;
        internal const int NUMBER_OF_TOP_GAMING_GROUPS_TO_SHOW = 15;
        internal const int NUMBER_OF_DAYS_OF_TRENDING_GAMES = 90;
        internal const int NUMBER_OF_TRENDING_GAMES_TO_SHOW = 5;

        private readonly IPlayerSummaryBuilder playerSummaryBuilder;
        private readonly ITopPlayerViewModelBuilder topPlayerViewModelBuilder;
        private readonly IPlayedGameRetriever playedGameRetriever;
        private readonly IGamingGroupRetriever gamingGroupRetriever;
        private readonly IGameDefinitionRetriever gameDefinitionRetriever;
        private readonly ITransformer transformer;

        public HomeController(
            IPlayerSummaryBuilder playerSummaryBuilder,
            ITopPlayerViewModelBuilder topPlayerViewModelBuilder,
            IPlayedGameRetriever playedGameRetriever,
            IGamingGroupRetriever gamingGroupRetriever,
            IGameDefinitionRetriever gameDefinitionRetriever,
            ITransformer transformer)
        {
            this.playerSummaryBuilder = playerSummaryBuilder;
            this.topPlayerViewModelBuilder = topPlayerViewModelBuilder;
            this.playedGameRetriever = playedGameRetriever;
            this.gamingGroupRetriever = gamingGroupRetriever;
            this.gameDefinitionRetriever = gameDefinitionRetriever;
            this.transformer = transformer;
        }

        public virtual ActionResult Index()
        {
            var topPlayers = playerSummaryBuilder.GetTopPlayers(NUMBER_OF_TOP_PLAYERS_TO_SHOW);
            var topPlayerViewModels = topPlayers.Select(
                topPlayer => this.topPlayerViewModelBuilder.Build(topPlayer)).ToList();

            var publicGameSummaries = playedGameRetriever
                .GetRecentPublicGames(NUMBER_OF_RECENT_PUBLIC_GAMES_TO_SHOW);

            var topGamingGroups = gamingGroupRetriever.GetTopGamingGroups(NUMBER_OF_TOP_GAMING_GROUPS_TO_SHOW);

            var topGamingGroupViewModels = topGamingGroups.Select(transformer.Transform<TopGamingGroupSummary, TopGamingGroupSummaryViewModel>).ToList();

            var trendingGames = gameDefinitionRetriever.GetTrendingGames(NUMBER_OF_TRENDING_GAMES_TO_SHOW, NUMBER_OF_DAYS_OF_TRENDING_GAMES);
            var trendingGameViewModels = trendingGames.Select(transformer.Transform<TrendingGame, TrendingGameViewModel>).ToList();

            var homeIndexViewModel = new HomeIndexViewModel()
            {
                TopPlayers = topPlayerViewModels,
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

        public virtual ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}