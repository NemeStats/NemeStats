using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UI.Controllers.Helpers;
using UI.Filters;
using UI.Models.GamingGroup;
using UI.Models.PlayedGame;
using UI.Transformations;

namespace UI.Controllers
{
    [Authorize]
    public partial class GamingGroupController : Controller
    {
        public const int MAX_NUMBER_OF_RECENT_GAMES = 10;
        public const string SECTION_ANCHOR_PLAYERS = "Players";
        public const string SECTION_ANCHOR_GAMEDEFINITIONS = "GameDefinitions";
        public const string SECTION_ANCHOR_RECENT_GAMES = "RecentGames";

        internal IDataContext dataContext;
        internal IGamingGroupViewModelBuilder gamingGroupViewModelBuilder;
        internal IGamingGroupAccessGranter gamingGroupAccessGranter;
        internal IGamingGroupCreator gamingGroupCreator;
        internal IGamingGroupRetriever gamingGroupRetriever;
        internal IShowingXResultsMessageBuilder showingXResultsMessageBuilder;

        public GamingGroupController(
            IDataContext dataContext,
            IGamingGroupViewModelBuilder gamingGroupViewModelBuilder,
            IGamingGroupAccessGranter gamingGroupAccessGranter,
            IGamingGroupCreator gamingGroupCreator,
            IGamingGroupRetriever gamingGroupRetriever,
            IShowingXResultsMessageBuilder showingXResultsMessageBuilder)
        {
            this.dataContext = dataContext;
            this.gamingGroupViewModelBuilder = gamingGroupViewModelBuilder;
            this.gamingGroupAccessGranter = gamingGroupAccessGranter;
            this.gamingGroupCreator = gamingGroupCreator;
            this.gamingGroupRetriever = gamingGroupRetriever;
            this.showingXResultsMessageBuilder = showingXResultsMessageBuilder;
        }

        // GET: /GamingGroup
        [UserContextAttribute]
        public virtual ActionResult Index(ApplicationUser currentUser)
        {
            GamingGroup gamingGroup = gamingGroupRetriever.GetGamingGroupDetails(
                currentUser.CurrentGamingGroupId.Value,
                MAX_NUMBER_OF_RECENT_GAMES);

            GamingGroupViewModel viewModel = gamingGroupViewModelBuilder.Build(gamingGroup, currentUser);

            ViewBag.RecentGamesSectionAnchorText = SECTION_ANCHOR_RECENT_GAMES;
            ViewBag.PlayerSectionAnchorText = SECTION_ANCHOR_PLAYERS;
            ViewBag.GameDefinitionSectionAnchorText = SECTION_ANCHOR_GAMEDEFINITIONS;

            ViewBag.RecentGamesMessage = showingXResultsMessageBuilder.BuildMessage(
                MAX_NUMBER_OF_RECENT_GAMES,
                gamingGroup.PlayedGames.Count);

            return View(MVC.GamingGroup.Views.Index, viewModel);
        }

        [HttpGet]
        [UserContextAttribute(RequiresGamingGroup = false)]
        public virtual ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [UserContextAttribute(RequiresGamingGroup = false)]
        public async virtual Task<ActionResult> Create(GamingGroupQuickStart gamingGroupQuickStart, ApplicationUser currentUser)
        {
            await gamingGroupCreator.CreateGamingGroupAsync(gamingGroupQuickStart, currentUser);
            return RedirectToAction(MVC.GamingGroup.ActionNames.Index);
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
    }
}
