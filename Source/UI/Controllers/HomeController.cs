using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Players;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UI.Models.Home;
using UI.Models.Nemeses;
using UI.Models.Players;
using UI.Transformations;
using UI.Transformations.PlayerTransformations;
using BusinessLogic.Logic.Nemeses;

namespace UI.Controllers
{
    public partial class HomeController : Controller
    {
        internal const int NUMBER_OF_TOP_PLAYERS_TO_SHOW = 5;
        internal const int NUMBER_OF_RECENT_PUBLIC_GAMES_TO_SHOW = 5;
        internal const int NUMBER_OF_RECENT_NEMESIS_CHANGES_TO_SHOW = 5;

        private readonly IPlayerSummaryBuilder playerSummaryBuilder;
        private readonly ITopPlayerViewModelBuilder topPlayerViewModelBuilder;
        private readonly IPlayedGameRetriever playedGameRetriever;
        private readonly INemesisHistoryRetriever nemesisHistoryRetriever;
        private readonly INemesisChangeViewModelBuilder nemesisChangeViewModelBuilder;

        public HomeController(
            IPlayerSummaryBuilder playerSummaryBuilder, 
            ITopPlayerViewModelBuilder topPlayerViewModelBuilder,
            IPlayedGameRetriever playedGameRetriever, 
            INemesisHistoryRetriever nemesisHistoryRetriever, INemesisChangeViewModelBuilder nemesisChangeViewModelBuilder)
        {
            this.playerSummaryBuilder = playerSummaryBuilder;
            this.topPlayerViewModelBuilder = topPlayerViewModelBuilder;
            this.playedGameRetriever = playedGameRetriever;
            this.nemesisHistoryRetriever = nemesisHistoryRetriever;
            this.nemesisChangeViewModelBuilder = nemesisChangeViewModelBuilder;
        }

        public virtual ActionResult Index()
        {
            List<TopPlayer> topPlayers = playerSummaryBuilder.GetTopPlayers(NUMBER_OF_TOP_PLAYERS_TO_SHOW);
            List<TopPlayerViewModel> topPlayerViewModels = topPlayers.Select(
                topPlayer => this.topPlayerViewModelBuilder.Build(topPlayer)).ToList();

            List<PublicGameSummary> publicGameSummaries = playedGameRetriever
                .GetRecentPublicGames(NUMBER_OF_RECENT_PUBLIC_GAMES_TO_SHOW);

            List<NemesisChange> nemesisChanges = nemesisHistoryRetriever.GetRecentNemesisChanges(NUMBER_OF_RECENT_NEMESIS_CHANGES_TO_SHOW);

            var nemesisChangeViewModels = nemesisChangeViewModelBuilder.Build(nemesisChanges);

            HomeIndexViewModel homeIndexViewModel = new HomeIndexViewModel()
            {
                TopPlayers = topPlayerViewModels,
                RecentPublicGames = publicGameSummaries,
                RecentNemesisChanges = nemesisChangeViewModels
            };
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