#region LICENSE

// NemeStats is a free website for tracking the results of board games. Copyright (C) 2015 Jacob Gordon
// 
// This program is free software: you can redistribute it and/or modify it under the terms of the
// GNU General Public License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with this program. If
// not, see <http://www.gnu.org/licenses/>

#endregion LICENSE

using AutoMapper;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.Logic;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UI.Attributes.Filters;
using UI.Controllers.Helpers;
using UI.Models.GamingGroup;
using UI.Models.PlayedGame;
using UI.Transformations;
using UI.Transformations.PlayerTransformations;

namespace UI.Controllers
{
	public partial class GamingGroupController : Controller
	{
		public const int MAX_NUMBER_OF_RECENT_GAMES = 10;
		public const int NUMBER_OF_TOP_GAMING_GROUPS_TO_SHOW = 25;
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
		[UserContext]
		public virtual ActionResult Index(ApplicationUser currentUser)
		{
			var gamingGroupSummary = this.GetGamingGroupSummary(currentUser.CurrentGamingGroupId.Value);

			GamingGroupViewModel viewModel = gamingGroupViewModelBuilder.Build(gamingGroupSummary, currentUser);
		    viewModel.PlayedGames.ShowSearchLinkInResultsHeader = true;

			ViewBag.RecentGamesSectionAnchorText = SECTION_ANCHOR_RECENT_GAMES;
			ViewBag.PlayerSectionAnchorText = SECTION_ANCHOR_PLAYERS;
			ViewBag.GameDefinitionSectionAnchorText = SECTION_ANCHOR_GAMEDEFINITIONS;

			return View(MVC.GamingGroup.Views.Index, viewModel);
		}

		internal virtual GamingGroupSummary GetGamingGroupSummary(int gamingGroupId)
		{
			GamingGroupSummary gamingGroupSummary = this.gamingGroupRetriever.GetGamingGroupDetails(
																							   gamingGroupId,
																							   MAX_NUMBER_OF_RECENT_GAMES);

			return gamingGroupSummary;
		}

		// GET: /GamingGroup/Details
		[UserContext(RequiresGamingGroup = false)]
		public virtual ActionResult Details(int id, ApplicationUser currentUser)
		{
			GamingGroupSummary gamingGroupSummary = GetGamingGroupSummary(id);

			var viewModel = new GamingGroupPublicViewModel
			{
				Id = gamingGroupSummary.Id,
				Name = gamingGroupSummary.Name,
				GameDefinitionSummaries = gamingGroupSummary.GameDefinitionSummaries
					.Select(summary => gameDefinitionSummaryViewModelBuilder.Build(summary, currentUser)).ToList(),
				Players = gamingGroupSummary.Players
					.Select(playerWithNemesis => playerWithNemesisViewModelBuilder.Build(playerWithNemesis, currentUser)).ToList(),
                PlayedGames = new PlayedGamesViewModel
                {
                    PlayedGameDetailsViewModels = gamingGroupSummary.PlayedGames
					.Select(playedGame => playedGameDetailsViewModelBuilder.Build(playedGame, currentUser)).ToList(),
                    PanelTitle = string.Format("Last {0} Played Games", gamingGroupSummary.PlayedGames.Count),
                    UserCanEdit = currentUser.CurrentGamingGroupId == gamingGroupSummary.Id,
                    ShowSearchLinkInResultsHeader = false
                    PanelTitle = string.Format("Last {0} Played Games", gamingGroupSummary.PlayedGames.Count)
                },
                PublicDetails = new GamingGroupPublicDetailsModel
                {
                    GamingGroupId = gamingGroupSummary.Id,
                    PublicDescription = gamingGroupSummary.PublicDescription,
                    Website = gamingGroupSummary.PublicGamingGroupWebsite
                }
			};

			return View(MVC.GamingGroup.Views.Details, viewModel);
		}

		[HttpGet]
		public virtual ActionResult GetTopGamingGroups()
		{
			var topGamingGroups = gamingGroupRetriever.GetTopGamingGroups(NUMBER_OF_TOP_GAMING_GROUPS_TO_SHOW);
			var topGamingGroupViewModels = topGamingGroups.Select(Mapper.Map<TopGamingGroupSummary, TopGamingGroupSummaryViewModel>).ToList();

			return View(MVC.GamingGroup.Views.TopGamingGroups, topGamingGroupViewModels);
		}

		[HttpPost]
		[Authorize]
		[UserContext]
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
		[UserContext]
        public virtual ActionResult UpdateGamingGroupName(string gamingGroupName, ApplicationUser currentUser)
		{
			try
			{
				gamingGroupSaver.UpdateGamingGroupName(gamingGroupName.Trim(), currentUser);
				cookieHelper.ClearCookie(NemeStatsCookieEnum.gamingGroupsCookie, Request, Response);

				return Json(new HttpStatusCodeResult(HttpStatusCode.OK));
			}
			catch (Exception)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotModified);
			}
		}

		[HttpPost]
		[Authorize]
		[UserContext]
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
		[UserContext]
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
		[UserContext]
		public virtual ActionResult CreateNewGamingGroup(string gamingGroupName, ApplicationUser currentUser)
		{
			if (string.IsNullOrWhiteSpace(gamingGroupName))
			{
				this.ModelState.AddModelError(string.Empty, "You must enter a Gaming Group name.");
				return this.Index(currentUser);
			}
			this.gamingGroupSaver.CreateNewGamingGroup(gamingGroupName.Trim(), TransactionSource.WebApplication, currentUser);
			this.cookieHelper.ClearCookie(NemeStatsCookieEnum.gamingGroupsCookie, this.Request, this.Response);

			return RedirectToAction(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

        [HttpGet]
        [Authorize]
        public virtual ActionResult Edit(int gamingGroupId)
        {
            var gamingGroup = gamingGroupRetriever.GetGamingGroupById(gamingGroupId);

            var model = new GamingGroupPublicDetailsModel
            {
                GamingGroupId = gamingGroupId,
                PublicDescription = gamingGroup.PublicDescription,
                Website = gamingGroup.PublicGamingGroupWebsite
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [UserContext]
        public virtual ActionResult Edit(GamingGroupEditRequest request, ApplicationUser curremUser)
        {
            this.gamingGroupSaver.UpdatePublicGamingGroupDetails(request, curremUser);

            return RedirectToAction(MVC.GamingGroup.Index());
        }
	}
}