using System.Web.Http;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UI.Controllers.Helpers;
using UI.Filters;
using UI.Models.Players;
using UI.Transformations;
using UI.Transformations.PlayerTransformations;

namespace UI.Controllers
{
    public partial class PlayerController : Controller
    {
        internal const int NUMBER_OF_RECENT_GAMES_TO_RETRIEVE = 10;
        internal const string EMAIL_SUBJECT_PLAYER_INVITATION = "Invitation from {0}";
        internal const string EMAIL_BODY_PLAYER_INVITATION = "Check out this gaming group I created to record the results of our board games!";

        internal IDataContext dataContext;
        internal IGameResultViewModelBuilder builder;
        internal IPlayerDetailsViewModelBuilder playerDetailsViewModelBuilder;
        internal IShowingXResultsMessageBuilder showingXResultsMessageBuilder;
        internal IPlayerSaver playerSaver;
        internal IPlayerRetriever playerRetriever;
        internal IPlayerInviter playerInviter;
        internal IPlayerEditViewModelBuilder playerEditViewModelBuilder;
        
        public PlayerController(IDataContext dataContext,
            IGameResultViewModelBuilder builder,
            IPlayerDetailsViewModelBuilder playerDetailsViewModelBuilder,
            IShowingXResultsMessageBuilder showingXResultsMessageBuilder,
            IPlayerSaver playerSaver,
            IPlayerRetriever playerRetriever,
            IPlayerInviter playerInviter,
            IPlayerEditViewModelBuilder playerEditViewModelBuilder)
        {
            this.dataContext = dataContext;
            this.builder = builder;
            this.playerDetailsViewModelBuilder = playerDetailsViewModelBuilder;
            this.showingXResultsMessageBuilder = showingXResultsMessageBuilder;
            this.playerSaver = playerSaver;
            this.playerRetriever = playerRetriever;
            this.playerInviter = playerInviter;
            this.playerEditViewModelBuilder = playerEditViewModelBuilder;
        }

        // GET: /Player/Details/5
        [UserContextAttribute(RequiresGamingGroup = false)]
        public virtual ActionResult Details(int? id, ApplicationUser currentUser)
        {
            if(!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PlayerDetails player = playerRetriever.GetPlayerDetails(id.Value, NUMBER_OF_RECENT_GAMES_TO_RETRIEVE);

            if (player == null)
            {
                return new HttpNotFoundResult();
            }

            PlayerDetailsViewModel playerDetailsViewModel = playerDetailsViewModelBuilder.Build(player,  currentUser);

            ViewBag.RecentGamesMessage = showingXResultsMessageBuilder.BuildMessage(
                NUMBER_OF_RECENT_GAMES_TO_RETRIEVE, 
                player.PlayerGameResults.Count);

            return View(MVC.Player.Views.Details, playerDetailsViewModel);
        }

        // GET: /Player/SavePlayer
        [System.Web.Mvc.Authorize]
        public virtual ActionResult SavePlayer()
        {
            return View(MVC.Player.Views._CreateOrUpdatePartial, new Player());
        }

        // GET: /Player/Create
        [System.Web.Mvc.Authorize]
        public virtual ActionResult Create()
        {
            return View(MVC.Player.Views.Create, new Player());
        }

        // GET: /Player/InvitePlayer/5
        [System.Web.Mvc.Authorize]
        [UserContextAttribute]
        public virtual ActionResult InvitePlayer(int id, ApplicationUser currentUser)
        {
            PlayerDetails playerDetails;

            try
            {
                playerDetails = playerRetriever.GetPlayerDetails(id, 0);
            }
            catch (KeyNotFoundException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            string emailSubject = string.Format(EMAIL_SUBJECT_PLAYER_INVITATION, currentUser.UserName);

            var playerInvitationViewModel = new PlayerInvitationViewModel
            {
                PlayerId = playerDetails.Id,
                PlayerName = playerDetails.Name,
                EmailSubject = emailSubject,
                EmailBody = EMAIL_BODY_PLAYER_INVITATION
            };
            return View(MVC.Player.Views.InvitePlayer, playerInvitationViewModel);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Authorize]
        [UserContextAttribute]
        public virtual ActionResult InvitePlayer(PlayerInvitationViewModel playerInvitationViewModel, ApplicationUser currentUser)
        {
            PlayerInvitation playerInvitation = new PlayerInvitation
            {
                InvitedPlayerId = playerInvitationViewModel.PlayerId,
                InvitedPlayerEmail = playerInvitationViewModel.EmailAddress,
                EmailSubject = playerInvitationViewModel.EmailSubject,
                CustomEmailMessage = playerInvitationViewModel.EmailBody
            };

            playerInviter.InvitePlayer(playerInvitation, currentUser);

            return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name));
        }

        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpPost]
        [UserContextAttribute]
        public virtual ActionResult Save(Player model, ApplicationUser currentUser)
        {
            if (!Request.IsAjaxRequest()) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if(ModelState.IsValid)
            {
                try
                {
                    Player player = playerSaver.Save(model, currentUser);
                    return Json(player, JsonRequestBehavior.AllowGet);
                }
                catch (PlayerAlreadyExistsException playerAlreadyExistsException)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict, playerAlreadyExistsException.Message);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotModified);
        }

        // GET: /Player/Edit/5
        [System.Web.Mvc.Authorize]
        public virtual ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlayerDetails player;
            try
            {
                player = playerRetriever.GetPlayerDetails(id.Value, 0);
            }catch(UnauthorizedAccessException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }catch(KeyNotFoundException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            var playerEditViewModel = new PlayerEditViewModel
            {
                Active = player.Active,
                Id = player.Id,
                GamingGroupId = player.GamingGroupId,
                Name = player.Name
            };
            return View(MVC.Player.Views.Edit, playerEditViewModel);
        }

        // POST: /Player/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [UserContextAttribute]
        public virtual ActionResult Edit([Bind(Include = "Id,Name,Active,GamingGroupId")] Player player, ApplicationUser currentUser)
        {
            if (ModelState.IsValid)
            {
                playerSaver.Save(player, currentUser);
                return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
                                          + "#" + GamingGroupController.SECTION_ANCHOR_PLAYERS);
            }

            var playerEditViewModel = playerEditViewModelBuilder.Build(player);

            return View(MVC.Player.Views.Edit, playerEditViewModel);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
