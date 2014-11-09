using System;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UI.Controllers.Helpers;
using UI.Filters;
using UI.Models.GamingGroup;
using UI.Transformations;
using UI.Transformations.PlayerTransformations;

namespace UI.Controllers
{
    [Authorize]
    public partial class GamingGroupController : Controller
    {
        public const int MAX_NUMBER_OF_RECENT_GAMES = 10;
        public const string SECTION_ANCHOR_PLAYERS = "Players";
        public const string SECTION_ANCHOR_GAMEDEFINITIONS = "GameDefinitions";
        public const string SECTION_ANCHOR_RECENT_GAMES = "RecentGames";

        internal IGamingGroupViewModelBuilder gamingGroupViewModelBuilder;
        internal IGamingGroupAccessGranter gamingGroupAccessGranter;
        internal IGamingGroupSaver gamingGroupSaver;
        internal IGamingGroupRetriever gamingGroupRetriever;
        internal IShowingXResultsMessageBuilder showingXResultsMessageBuilder;
        internal IPlayerWithNemesisViewModelBuilder playerWithNemesisViewModelBuilder;
        internal IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder;

        public GamingGroupController(
            IGamingGroupViewModelBuilder gamingGroupViewModelBuilder,
            IGamingGroupAccessGranter gamingGroupAccessGranter,
            IGamingGroupSaver gamingGroupSaver,
            IGamingGroupRetriever gamingGroupRetriever,
            IShowingXResultsMessageBuilder showingXResultsMessageBuilder,
            IPlayerWithNemesisViewModelBuilder playerWithNemesisViewModelBuilder,
            IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder)
        {
            this.gamingGroupViewModelBuilder = gamingGroupViewModelBuilder;
            this.gamingGroupAccessGranter = gamingGroupAccessGranter;
            this.gamingGroupSaver = gamingGroupSaver;
            this.gamingGroupRetriever = gamingGroupRetriever;
            this.showingXResultsMessageBuilder = showingXResultsMessageBuilder;
            this.playerWithNemesisViewModelBuilder = playerWithNemesisViewModelBuilder;
            this.playedGameDetailsViewModelBuilder = playedGameDetailsViewModelBuilder;
        }

        // GET: /GamingGroup
        [UserContextAttribute]
        public virtual ActionResult Index(ApplicationUser currentUser)
        {
            GamingGroupSummary gamingGroupSummary = gamingGroupRetriever.GetGamingGroupDetails(
                currentUser.CurrentGamingGroupId.Value,
                MAX_NUMBER_OF_RECENT_GAMES);

            GamingGroupViewModel viewModel = gamingGroupViewModelBuilder.Build(gamingGroupSummary, currentUser);

            ViewBag.RecentGamesSectionAnchorText = SECTION_ANCHOR_RECENT_GAMES;
            ViewBag.PlayerSectionAnchorText = SECTION_ANCHOR_PLAYERS;
            ViewBag.GameDefinitionSectionAnchorText = SECTION_ANCHOR_GAMEDEFINITIONS;

            ViewBag.RecentGamesMessage = showingXResultsMessageBuilder.BuildMessage(
                MAX_NUMBER_OF_RECENT_GAMES,
                gamingGroupSummary.PlayedGames.Count);

            return View(MVC.GamingGroup.Views.Index, viewModel);
        }

        // GET: /GamingGroup/Details
        [UserContextAttribute]
        public virtual ActionResult Details(ApplicationUser currentUser)
        {
            GamingGroupSummary gamingGroupSummary = gamingGroupRetriever.GetGamingGroupDetails(
                currentUser.CurrentGamingGroupId.Value,
                MAX_NUMBER_OF_RECENT_GAMES);

            GamingGroupPublicViewModel viewModel = new GamingGroupPublicViewModel
            {
                Id = gamingGroupSummary.Id,
                Name = gamingGroupSummary.Name,
                GameDefinitionSummaries = gamingGroupSummary.GameDefinitionSummaries,
                Players = gamingGroupSummary.Players
                    .Select(player => playerWithNemesisViewModelBuilder.Build(player)).ToList(),
                RecentGames = gamingGroupSummary.PlayedGames
                    .Select(playedGame => playedGameDetailsViewModelBuilder.Build(playedGame, currentUser)).ToList()
            };

            ViewBag.RecentGamesMessage = showingXResultsMessageBuilder.BuildMessage(
                MAX_NUMBER_OF_RECENT_GAMES,
                gamingGroupSummary.PlayedGames.Count);

            return View(MVC.GamingGroup.Views.Details, viewModel);
        }

        //
        // POST: /GamingGroup/Delete/5
        [HttpPost]
        [UserContextAttribute]
        public virtual ActionResult GrantAccess(GamingGroupViewModel model, ApplicationUser currentUser)
        {
            if (ModelState.IsValid)
            {
                gamingGroupAccessGranter.CreateInvitation(model.InviteeEmail, currentUser);
                return RedirectToAction(MVC.GamingGroup.ActionNames.Index);
            }

            return RedirectToAction(MVC.GamingGroup.ActionNames.Index, model);
        }

        [HttpPost]
        [UserContextAttribute]
        public virtual ActionResult Edit(string gamingGroupName, ApplicationUser currentUser)
        {
            try
            {
                gamingGroupSaver.UpdateGamingGroupName(gamingGroupName, currentUser);
                return Json(new HttpStatusCodeResult(HttpStatusCode.OK));
            }
            catch(Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotModified);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
