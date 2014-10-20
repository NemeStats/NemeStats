using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UI.Controllers.Helpers;
using UI.Filters;
using UI.Models.GamingGroup;
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

        internal IGamingGroupViewModelBuilder gamingGroupViewModelBuilder;
        internal IGamingGroupAccessGranter gamingGroupAccessGranter;
        internal IGamingGroupSaver gamingGroupSaver;
        internal IGamingGroupRetriever gamingGroupRetriever;
        internal IShowingXResultsMessageBuilder showingXResultsMessageBuilder;

        public GamingGroupController(
            IGamingGroupViewModelBuilder gamingGroupViewModelBuilder,
            IGamingGroupAccessGranter gamingGroupAccessGranter,
            IGamingGroupSaver gamingGroupSaver,
            IGamingGroupRetriever gamingGroupRetriever,
            IShowingXResultsMessageBuilder showingXResultsMessageBuilder)
        {
            this.gamingGroupViewModelBuilder = gamingGroupViewModelBuilder;
            this.gamingGroupAccessGranter = gamingGroupAccessGranter;
            this.gamingGroupSaver = gamingGroupSaver;
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
            gamingGroupSaver.UpdateGamingGroupName(gamingGroupName, currentUser);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
