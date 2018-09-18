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
#endregion

using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UI.Attributes.Filters;
using UI.Controllers.Helpers;
using UI.Models.Players;
using UI.Transformations;
using UI.Transformations.PlayerTransformations;
using AutoMapper;
using BusinessLogic.Models.Nemeses;

namespace UI.Controllers
{
    public partial class PlayerController : BaseController
    {
        internal const int NUMBER_OF_RECENT_GAMES_TO_RETRIEVE = 10;
        internal const int NUMBER_OF_TOP_PLAYERS_TO_RETRIEVE = 25;
        internal const int NUMBER_OF_RECENT_NEMESIS_CHANGES_TO_RETRIEVE = 25;
        internal const string EMAIL_SUBJECT_PLAYER_INVITATION = "Invitation from {0}";
        internal const string EMAIL_BODY_PLAYER_INVITATION = "Check out this gaming group I created to record the results of our board games!";

        internal IGameResultViewModelBuilder builder;
        internal IPlayerDetailsViewModelBuilder playerDetailsViewModelBuilder;
        internal IShowingXResultsMessageBuilder showingXResultsMessageBuilder;
        internal IPlayerSaver playerSaver;
        internal IPlayerRetriever playerRetriever;
        internal IPlayerInviter playerInviter;
        internal IPlayerEditViewModelBuilder playerEditViewModelBuilder;
        internal IPlayerSummaryBuilder playerSummaryBuilder;
        internal ITopPlayerViewModelBuilder topPlayerViewModelBuilder;
        internal INemesisHistoryRetriever nemesisHistoryRetriever;
        internal INemesisChangeViewModelBuilder nemesisChangeViewModelBuilder;
        private readonly IPlayerDeleter _playerDeleter;
        private readonly IHomePagePlayerSummaryRetriever _homePagePlayerSummaryRetriever;

        public PlayerController(
            IGameResultViewModelBuilder builder,
            IPlayerDetailsViewModelBuilder playerDetailsViewModelBuilder,
            IShowingXResultsMessageBuilder showingXResultsMessageBuilder,
            IPlayerSaver playerSaver,
            IPlayerRetriever playerRetriever,
            IPlayerInviter playerInviter,
            IPlayerEditViewModelBuilder playerEditViewModelBuilder,
            IPlayerSummaryBuilder playerSummaryBuilder,
            ITopPlayerViewModelBuilder topPlayerViewModelBuilder,
            INemesisHistoryRetriever nemesisHistoryRetriever,
            INemesisChangeViewModelBuilder nemesisChangeViewModelBuilder,
            IPlayerDeleter playerDeleter, 
            IHomePagePlayerSummaryRetriever homePagePlayerSummaryRetriever)
        {
            this.builder = builder;
            this.playerDetailsViewModelBuilder = playerDetailsViewModelBuilder;
            this.showingXResultsMessageBuilder = showingXResultsMessageBuilder;
            this.playerSaver = playerSaver;
            this.playerRetriever = playerRetriever;
            this.playerInviter = playerInviter;
            this.playerEditViewModelBuilder = playerEditViewModelBuilder;
            this.playerSummaryBuilder = playerSummaryBuilder;
            this.topPlayerViewModelBuilder = topPlayerViewModelBuilder;
            this.nemesisHistoryRetriever = nemesisHistoryRetriever;
            this.nemesisChangeViewModelBuilder = nemesisChangeViewModelBuilder;
            _playerDeleter = playerDeleter;
            _homePagePlayerSummaryRetriever = homePagePlayerSummaryRetriever;
        }

        // GET: /Player/Details/5
        [UserContext(RequiresGamingGroup = false)]
        public virtual ActionResult Details(int id, ApplicationUser currentUser)
        {
            var player = playerRetriever.GetPlayerDetails(id, NUMBER_OF_RECENT_GAMES_TO_RETRIEVE);
            
            var fullUrl = Url.Action(MVC.Player.ActionNames.Details, MVC.Player.Name, new { id }, Request.Url.Scheme) + "#minions";

            var playerIds = player.PlayerVersusPlayersStatistics.Select(x => x.OpposingPlayerId).ToList();
            //--include the current player so we can attempt to get their email address as well
            playerIds.Add(id);
            var playerIdToRegisteredUserEmailAddressDictionary =
                playerRetriever.GetRegisteredUserEmailAddresses(playerIds, currentUser);
            var playerDetailsViewModel = playerDetailsViewModelBuilder.Build(player, playerIdToRegisteredUserEmailAddressDictionary, fullUrl, currentUser);

            ViewBag.RecentGamesMessage = showingXResultsMessageBuilder.BuildMessage(
                NUMBER_OF_RECENT_GAMES_TO_RETRIEVE,
                player.PlayerGameResults.Count);

            return View(MVC.Player.Views.Details, playerDetailsViewModel);
        }

        // GET: /Player/SavePlayer
        [Authorize]
        public virtual ActionResult SavePlayer()
        {
            return View(MVC.Player.Views._CreateOrUpdatePartial, new Player());
        }

        // GET: /Player/Create
        [Authorize]
        public virtual ActionResult Create()
        {
            return View(MVC.Player.Views.Create, new Player());
        }

        // GET: /Player/InvitePlayer/5
        [Authorize]
        [UserContext]
        public virtual ActionResult InvitePlayer(int id, ApplicationUser currentUser)
        {
            PlayerDetails playerDetails;

            try
            {
                //TODO this method is overkill for just getting the player name
                playerDetails = playerRetriever.GetPlayerDetails(id, 0);
            }
            catch (KeyNotFoundException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var emailSubject = string.Format(EMAIL_SUBJECT_PLAYER_INVITATION, currentUser.UserName);

            var playerInvitationViewModel = new PlayerInvitationViewModel
            {
                PlayerId = playerDetails.Id,
                PlayerName = playerDetails.Name,
                EmailSubject = emailSubject,
                EmailBody = EMAIL_BODY_PLAYER_INVITATION
            };
            return View(MVC.Player.Views.InvitePlayer, playerInvitationViewModel);
        }

        [HttpGet]
        public virtual ActionResult ShowTopPlayers()
        {
            var topPlayers = playerSummaryBuilder.GetTopPlayers(NUMBER_OF_TOP_PLAYERS_TO_RETRIEVE);
            var topPlayersViewModels = topPlayers.Select(topPlayer => topPlayerViewModelBuilder.Build(topPlayer)).ToList();
            return View(MVC.Player.Views.TopPlayers, topPlayersViewModels);
        }

        [HttpGet]
        public virtual ActionResult ShowRecentNemesisChanges()
        {
            var request = new GetRecentNemesisChangesRequest
            {
                NumberOfRecentChangesToRetrieve = NUMBER_OF_RECENT_NEMESIS_CHANGES_TO_RETRIEVE
            };
            var recentNemesisChanges = nemesisHistoryRetriever.GetRecentNemesisChanges(request);
            var recentNemesisChangesViewModels = nemesisChangeViewModelBuilder.Build(recentNemesisChanges).ToList();
            return View(MVC.Player.Views.RecentNemesisChanges, recentNemesisChangesViewModels);
        }

        [HttpPost]
        [Authorize]
        [UserContext]
        public virtual ActionResult InvitePlayer(PlayerInvitationViewModel playerInvitationViewModel, ApplicationUser currentUser)
        {
            var playerInvitation = new PlayerInvitation
            {
                InvitedPlayerId = playerInvitationViewModel.PlayerId,
                InvitedPlayerEmail = playerInvitationViewModel.EmailAddress.Trim(),
                EmailSubject = playerInvitationViewModel.EmailSubject,
                CustomEmailMessage = playerInvitationViewModel.EmailBody,
                GamingGroupId = currentUser.CurrentGamingGroupId.Value
            };

            playerInviter.InvitePlayer(playerInvitation, currentUser);

            SetToastMessage(TempMessageKeys.TEMP_MESSAGE_KEY_PLAYER_INVITED,$"Email invitation successfully sent to {playerInvitationViewModel.PlayerName}!");

            return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name));
        }

        [Authorize]
        [HttpPost]
        [UserContext]
        public virtual ActionResult Save(CreatePlayerRequest createPlayerRequest, ApplicationUser currentUser)
        {
            if (!Request.IsAjaxRequest())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    createPlayerRequest.Name = createPlayerRequest.Name.Trim();
                    createPlayerRequest.GamingGroupId = currentUser.CurrentGamingGroupId;
                    var player = playerSaver.CreatePlayer(createPlayerRequest, currentUser);
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
        [Authorize]
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
            }
            catch (UnauthorizedAccessException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            catch (KeyNotFoundException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            var playerEditViewModel = new PlayerEditViewModel
            {
                Active = player.Active,
                Id = player.Id,
                GamingGroupId = player.GamingGroupId,
                Name = player.Name,
                IsDeleteable = !player.PlayerGameSummaries.Any() && string.IsNullOrEmpty(player.ApplicationUserId)
            };
            return View(MVC.Player.Views.Edit, playerEditViewModel);
        }

        // POST: /Player/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserContext]
        public virtual ActionResult Edit([Bind(Include = "Id,Name,Active,GamingGroupId")] Player player, ApplicationUser currentUser)
        {
            if (ModelState.IsValid)
            {
                player.Name = player.Name.Trim();

                var requestedPlayer = new UpdatePlayerRequest
                {
                    PlayerId = player.Id,
                    Active = player.Active,
                    Name = player.Name,
                };

                try
                {
                    playerSaver.UpdatePlayer(requestedPlayer, currentUser);
                    return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
                                              + "#" + GamingGroupController.SECTION_ANCHOR_PLAYERS);
                }
                catch (PlayerAlreadyExistsException)
                {
                    ModelState.AddModelError(string.Empty,
                        $@"A Player with name '{requestedPlayer.Name}' already exists in this Gaming Group. Choose another.");
                }
            }

            var playerEditViewModel = playerEditViewModelBuilder.Build(player);

            return View(MVC.Player.Views.Edit, playerEditViewModel);
        }

        // POST: /Player/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserContext]
        public virtual ActionResult Delete(int id, ApplicationUser currentUser)
        {
            _playerDeleter.DeletePlayer(id, currentUser);
            
            SetToastMessage(TempMessageKeys.TEMP_MESSAGE_KEY_PLAYER_DELETED, "Player deleted successfully");

            return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name));
        }

        [Authorize]
        [UserContext]
        public virtual ActionResult CurrentPlayerQuickStats(ApplicationUser currentUser)
        {
            var homePagePlayerSummary = _homePagePlayerSummaryRetriever.GetHomePagePlayerSummaryForUser(currentUser.Id, currentUser.CurrentGamingGroupId.Value);

            var playerQuickStatsViewModel = Mapper.Map<PlayerQuickStatsViewModel>(homePagePlayerSummary);

            return PartialView(MVC.Player.Views._CurrentPlayerQuickStatsPartial, playerQuickStatsViewModel);
        }
    }
}
