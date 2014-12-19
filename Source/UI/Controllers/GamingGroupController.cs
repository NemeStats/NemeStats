using System;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Users;
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
        internal IGameDefinitionSummaryViewModelBuilder gameDefinitionSummaryViewModelBuilder;
        internal IGamingGroupContextSwitcher gamingGroupContextSwitcher;
        internal ICookieHelper cookieHelper;

        public GamingGroupController(
            IGamingGroupViewModelBuilder gamingGroupViewModelBuilder,
            IGamingGroupAccessGranter gamingGroupAccessGranter,
            IGamingGroupSaver gamingGroupSaver,
            IGamingGroupRetriever gamingGroupRetriever,
            IShowingXResultsMessageBuilder showingXResultsMessageBuilder,
            IPlayerWithNemesisViewModelBuilder playerWithNemesisViewModelBuilder,
            IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder,
            IGameDefinitionSummaryViewModelBuilder gameDefinitionSummaryViewModelBuilder,
            IGamingGroupContextSwitcher gamingGroupContextSwitcher,
            ICookieHelper cookieHelper)
        {
            this.gamingGroupViewModelBuilder = gamingGroupViewModelBuilder;
            this.gamingGroupAccessGranter = gamingGroupAccessGranter;
            this.gamingGroupSaver = gamingGroupSaver;
            this.gamingGroupRetriever = gamingGroupRetriever;
            this.showingXResultsMessageBuilder = showingXResultsMessageBuilder;
            this.playerWithNemesisViewModelBuilder = playerWithNemesisViewModelBuilder;
            this.playedGameDetailsViewModelBuilder = playedGameDetailsViewModelBuilder;
            this.gameDefinitionSummaryViewModelBuilder = gameDefinitionSummaryViewModelBuilder;
            this.gamingGroupContextSwitcher = gamingGroupContextSwitcher;
            this.cookieHelper = cookieHelper;
        }

        // GET: /GamingGroup
        [Authorize]
        [UserContextAttribute]
        public virtual ActionResult Index(ApplicationUser currentUser)
        {
            var gamingGroupSummary = this.GetGamingGroupSummaryAndSetRecentGamesMessage(currentUser.CurrentGamingGroupId.Value);

            GamingGroupViewModel viewModel = gamingGroupViewModelBuilder.Build(gamingGroupSummary, currentUser);

            ViewBag.RecentGamesSectionAnchorText = SECTION_ANCHOR_RECENT_GAMES;
            ViewBag.PlayerSectionAnchorText = SECTION_ANCHOR_PLAYERS;
            ViewBag.GameDefinitionSectionAnchorText = SECTION_ANCHOR_GAMEDEFINITIONS;

            return View(MVC.GamingGroup.Views.Index, viewModel);
        }

        internal virtual GamingGroupSummary GetGamingGroupSummaryAndSetRecentGamesMessage(int gamingGroupId)
        {
            GamingGroupSummary gamingGroupSummary = this.gamingGroupRetriever.GetGamingGroupDetails(
                                                                                               gamingGroupId,
                                                                                               MAX_NUMBER_OF_RECENT_GAMES);

            this.ViewBag.RecentGamesMessage = this.showingXResultsMessageBuilder.BuildMessage(
                                                                                    MAX_NUMBER_OF_RECENT_GAMES,
                                                                                    gamingGroupSummary.PlayedGames.Count);
            return gamingGroupSummary;
        }

        // GET: /GamingGroup/Details
        [UserContextAttribute(RequiresGamingGroup = false)]
        public virtual ActionResult Details(int id, ApplicationUser currentUser)
        {
            GamingGroupSummary gamingGroupSummary = GetGamingGroupSummaryAndSetRecentGamesMessage(id);

            GamingGroupPublicViewModel viewModel = new GamingGroupPublicViewModel
            {
                Id = gamingGroupSummary.Id,
                Name = gamingGroupSummary.Name,
                GameDefinitionSummaries = gamingGroupSummary.GameDefinitionSummaries
                    .Select(summary => gameDefinitionSummaryViewModelBuilder.Build(summary, currentUser)).ToList(),
                Players = gamingGroupSummary.Players
                    .Select(playerWithNemesis => playerWithNemesisViewModelBuilder.Build(playerWithNemesis, currentUser)).ToList(),
                RecentGames = gamingGroupSummary.PlayedGames
                    .Select(playedGame => playedGameDetailsViewModelBuilder.Build(playedGame, currentUser)).ToList()
            };

            return View(MVC.GamingGroup.Views.Details, viewModel);
        }

        [HttpPost]
        [Authorize]
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
        [Authorize]
        [UserContextAttribute]
        public virtual ActionResult Edit(string gamingGroupName, ApplicationUser currentUser)
        {
            try
            {
                gamingGroupSaver.UpdateGamingGroupName(gamingGroupName, currentUser);
                cookieHelper.ClearCookie(NemeStatsCookieEnum.gamingGroupsCookie, Request, Response);

                return Json(new HttpStatusCodeResult(HttpStatusCode.OK));
            }
            catch(Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotModified);
            }
        }

        [HttpPost]
        [Authorize]
        [UserContextAttribute]
        public virtual ActionResult GetUsersGamingGroups(ApplicationUser currentUser)
        {
            var gamingGroups = gamingGroupRetriever.GetGamingGroupsForUser(currentUser);
            JsonResult jsonResult = new JsonResult();
            var gamingGroupList = (from gamingGroup in gamingGroups
                                   select new
                                   {
                                       gamingGroup.Id,
                                       gamingGroup.Name,
                                       IsCurrentGamingGroup = currentUser.CurrentGamingGroupId == gamingGroup.Id
                                   }).ToList();

            return Json(gamingGroupList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        [UserContextAttribute]
        public virtual ActionResult SwitchGamingGroups(int gamingGroupId, ApplicationUser currentUser)
        {
            if (gamingGroupId != currentUser.CurrentGamingGroupId)
            {
                gamingGroupContextSwitcher.SwitchGamingGroupContext(gamingGroupId, currentUser);
            }

            return RedirectToAction(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name);
        }

        [HttpPost]
        [Authorize]
        [UserContextAttribute]
        public virtual ActionResult CreateNewGamingGroup(string gamingGroupName, ApplicationUser currentUser)
        {
            if (string.IsNullOrWhiteSpace(gamingGroupName))
            {
                this.ModelState.AddModelError(string.Empty, "You must enter a Gaming Group name.");
                return this.Index(currentUser);
            }
            else
            {
                gamingGroupSaver.CreateNewGamingGroup(gamingGroupName.Trim(), currentUser);
                cookieHelper.ClearCookie(NemeStatsCookieEnum.gamingGroupsCookie, Request, Response);
            }

            return RedirectToAction(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
