using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UI.Models.Home;
using UI.Models.Players;
using UI.Transformations.Player;

namespace UI.Controllers
{
    public partial class HomeController : Controller
    {
        internal const int NUMBER_OF_TOP_PLAYERS_TO_SHOW = 5;
        internal const int NUMBER_OF_RECENT_PUBLIC_GAMES_TO_SHOW = 5;

        private IPlayerSummaryBuilder playerSummaryBuilder;
        private TopPlayerViewModelBuilder topPlayerViewModelBuilder;
        private IPlayedGameRetriever playedGameRetriever;

        public HomeController(
            IPlayerSummaryBuilder playerSummaryBuilder, 
            TopPlayerViewModelBuilder topPlayerViewModelBuilder,
            IPlayedGameRetriever playedGameRetriever)
        {
            this.playerSummaryBuilder = playerSummaryBuilder;
            this.topPlayerViewModelBuilder = topPlayerViewModelBuilder;
            this.playedGameRetriever = playedGameRetriever;
        }

        public virtual ActionResult Index()
        {
            List<TopPlayer> topPlayers = playerSummaryBuilder.GetTopPlayers(NUMBER_OF_TOP_PLAYERS_TO_SHOW);
            List<TopPlayerViewModel> topPlayerViewModels = new List<TopPlayerViewModel>();
            foreach(TopPlayer topPlayer in topPlayers)
            {
                topPlayerViewModels.Add(topPlayerViewModelBuilder.Build(topPlayer));
            }

            List<PublicGameSummary> publicGameSummaries = playedGameRetriever
                .GetRecentPublicGames(NUMBER_OF_RECENT_PUBLIC_GAMES_TO_SHOW);

            HomeIndexViewModel homeIndexViewModel = new HomeIndexViewModel()
            {
                TopPlayers = topPlayerViewModels,
                RecentPublicGames = publicGameSummaries
            };
            return View(MVC.Home.Views.Index, homeIndexViewModel);
        }

        public virtual ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public virtual ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}