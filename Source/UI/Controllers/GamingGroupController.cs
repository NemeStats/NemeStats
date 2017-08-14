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

using System.Collections.Generic;
using AutoMapper;
using BusinessLogic.Logic;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using BusinessLogic.Models.Utility;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using UI.Attributes.Filters;
using UI.Controllers.Helpers;
using UI.Models.GamingGroup;
using UI.Models.PlayedGame;
using UI.Models.Players;
using UI.Transformations;
using UI.Transformations.PlayerTransformations;

namespace UI.Controllers
{
    public partial class GamingGroupController : BaseController
    {
        public const int MAX_NUMBER_OF_RECENT_GAMES = 10;
        public const int NUMBER_OF_TOP_GAMING_GROUPS_TO_SHOW = 25;
        public const string SECTION_ANCHOR_PLAYERS = "playersListDivId";
        public const string SECTION_ANCHOR_GAMEDEFINITIONS = "gamesListDivId";
        public const string SECTION_ANCHOR_RECENT_GAMES = "playedGamesListDivId";

        internal IGamingGroupSaver gamingGroupSaver;
        internal IGamingGroupRetriever gamingGroupRetriever;
        internal IPlayerWithNemesisViewModelBuilder playerWithNemesisViewModelBuilder;
        internal IGameDefinitionSummaryViewModelBuilder gameDefinitionSummaryViewModelBuilder;
        internal IGamingGroupContextSwitcher gamingGroupContextSwitcher;
        internal IPlayerRetriever playerRetriever;
        internal IGameDefinitionRetriever gameDefinitionRetriever;
        internal IPlayedGameRetriever playedGameRetriever;
        internal IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder;

        public GamingGroupController(
            IGamingGroupSaver gamingGroupSaver,
            IGamingGroupRetriever gamingGroupRetriever,
            IPlayerWithNemesisViewModelBuilder playerWithNemesisViewModelBuilder,
            IGameDefinitionSummaryViewModelBuilder gameDefinitionSummaryViewModelBuilder,
            IGamingGroupContextSwitcher gamingGroupContextSwitcher,
            IPlayerRetriever playerRetriever, 
            IGameDefinitionRetriever gameDefinitionRetriever, 
            IPlayedGameRetriever playedGameRetriever, 
            IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder)
        {
            this.gamingGroupSaver = gamingGroupSaver;
            this.gamingGroupRetriever = gamingGroupRetriever;
            this.playerWithNemesisViewModelBuilder = playerWithNemesisViewModelBuilder;
            this.gameDefinitionSummaryViewModelBuilder = gameDefinitionSummaryViewModelBuilder;
            this.gamingGroupContextSwitcher = gamingGroupContextSwitcher;
            this.playerRetriever = playerRetriever;
            this.gameDefinitionRetriever = gameDefinitionRetriever;
            this.playedGameRetriever = playedGameRetriever;
            this.playedGameDetailsViewModelBuilder = playedGameDetailsViewModelBuilder;
        }

        // GET: /GamingGroup
        [Authorize]
        [UserContext]
        public virtual ActionResult Index(ApplicationUser currentUser)
        {
            return RedirectToAction(MVC.GamingGroup.ActionNames.Details, new { id = currentUser.CurrentGamingGroupId });
        }

        [UserContext(RequiresGamingGroup = false)]
        public virtual ActionResult Details(int id, ApplicationUser currentUser, [System.Web.Http.FromUri]BasicDateRangeFilter dateRangeFilter = null)
        {
            if (dateRangeFilter == null)
            {
                dateRangeFilter = new BasicDateRangeFilter();
            }
            string errorMessage;
            if (!dateRangeFilter.IsValid(out errorMessage))
            {
                ModelState.AddModelError("dateRangeFilter", errorMessage);
            }

            var gamingGroupSummary = GetGamingGroupSummary(id, dateRangeFilter);
            var viewModel = new GamingGroupViewModel
            {
                PublicDetailsView = new GamingGroupPublicDetailsViewModel
                {
                    GamingGroupId = gamingGroupSummary.Id,
                    GamingGroupName = gamingGroupSummary.Name,
                    PublicDescription = gamingGroupSummary.PublicDescription,
                    Website = gamingGroupSummary.PublicGamingGroupWebsite
                }
            };
            viewModel.DateRangeFilter = dateRangeFilter;
            viewModel.UserCanEdit = currentUser.CurrentGamingGroupId == id;
            //viewModel.ShowResultsInHeader

            return View(MVC.GamingGroup.Views.Details, viewModel);
        }

        [NonAction]
        internal virtual GamingGroupSummary GetGamingGroupSummary(int gamingGroupId, IDateRangeFilter dateRangeFilter = null)
        {
            if (dateRangeFilter == null)
            {
                dateRangeFilter = new BasicDateRangeFilter();
            }
            else
            {
                dateRangeFilter.FromDate = dateRangeFilter.FromDate;
                dateRangeFilter.ToDate = dateRangeFilter.ToDate;
            }

            var filter = new GamingGroupFilter(dateRangeFilter)
            {
                NumberOfRecentGamesToShow = MAX_NUMBER_OF_RECENT_GAMES,
                GamingGroupId = gamingGroupId
            };

            return gamingGroupRetriever.GetGamingGroupDetails(filter);
        }

        [HttpGet]
        [UserContext(RequiresGamingGroup = false)]
        public virtual ActionResult GetGamingGroupPlayers(int id, ApplicationUser currentUser, [System.Web.Http.FromUri]BasicDateRangeFilter dateRangeFilter = null)
        {
            var playersWithNemesis = playerRetriever.GetAllPlayersWithNemesisInfo(id, dateRangeFilter)
                .Select(player => playerWithNemesisViewModelBuilder.Build(player, currentUser))
                .ToList();
            return View(MVC.Player.Views._PlayersPartial, playersWithNemesis);
        }

        [HttpGet]
        [UserContext(RequiresGamingGroup = false)]
        public virtual ActionResult GetGamingGroupGameDefinitions(int id, ApplicationUser currentUser, [System.Web.Http.FromUri]BasicDateRangeFilter dateRangeFilter = null)
        {
            var games =
                gameDefinitionRetriever.GetAllGameDefinitions(id, dateRangeFilter)
                    .Select(gameDefinition => gameDefinitionSummaryViewModelBuilder.Build(gameDefinition, currentUser))
                    .OrderByDescending(x => x.TotalNumberOfGamesPlayed)
                    .ThenBy(x => x.Name)
                    .ToList();
            return View(MVC.GameDefinition.Views._GameDefinitionsPartial, games);
        }

        [HttpGet]
        [UserContext(RequiresGamingGroup = false)]
        public virtual ActionResult GetGamingGroupPlayedGames(int id, ApplicationUser currentUser, [System.Web.Http.FromUri]BasicDateRangeFilter dateRangeFilter = null, [System.Web.Http.FromUri]int numberOfItems = 10)
        {
            var games = playedGameRetriever.GetRecentGames(numberOfItems, id, dateRangeFilter);
            var viewModel = new PlayedGamesViewModel
            {
                GamingGroupId = id,
                ShowSearchLinkInResultsHeader = true,
                PlayedGameDetailsViewModels = games.Select(playedGame => playedGameDetailsViewModelBuilder.Build(playedGame, currentUser)).ToList()
            };

            return View(MVC.PlayedGame.Views._PlayedGamesPartial, viewModel);
        }

        [HttpGet]
        public virtual ActionResult GetTopGamingGroups()
        {
            var topGamingGroups = gamingGroupRetriever.GetTopGamingGroups(NUMBER_OF_TOP_GAMING_GROUPS_TO_SHOW);
            var topGamingGroupViewModels = topGamingGroups.Select(Mapper.Map<TopGamingGroupSummary, TopGamingGroupSummaryViewModel>).ToList();

            return View(MVC.GamingGroup.Views.TopGamingGroups, topGamingGroupViewModels);
        }

        [HttpGet]
        [UserContext]
        public virtual ActionResult GetCurrentUserGamingGroupGameDefinitions(int id, ApplicationUser currentUser)
        {
            var model = gameDefinitionRetriever.GetAllGameDefinitions(id)
                .Select(summary => gameDefinitionSummaryViewModelBuilder.Build(summary, currentUser)).ToList();

            ViewData["canEdit"] = true;

            return View(MVC.GameDefinition.Views._GameDefinitionsTablePartial, model);
        }

        [Authorize]
        [UserContext]
        public virtual ActionResult SwitchGamingGroups(int gamingGroupId, ApplicationUser currentUser)
        {
            if (gamingGroupId != currentUser.CurrentGamingGroupId)
            {
                gamingGroupContextSwitcher.SwitchGamingGroupContext(gamingGroupId, currentUser);
            }

            return RedirectToAction(MVC.GamingGroup.Details().AddRouteValue("id", gamingGroupId));
        }

        [HttpPost]
        [Authorize]
        [UserContext]
        public virtual ActionResult CreateNewGamingGroup(string gamingGroupName, ApplicationUser currentUser)
        {
            if (string.IsNullOrWhiteSpace(gamingGroupName))
            {
                this.ModelState.AddModelError(string.Empty, "You must enter a Gaming Group name.");
                return this.Details(currentUser.CurrentGamingGroupId, currentUser);
            }
            this.gamingGroupSaver.CreateNewGamingGroup(gamingGroupName.Trim(), TransactionSource.WebApplication, currentUser);

            return RedirectToAction(MVC.GamingGroup.ActionNames.Details, new {id = currentUser.CurrentGamingGroupId } );
        }

        [HttpGet]
        [Authorize]
        public virtual ActionResult Edit(int id)
        {
            var gamingGroup = gamingGroupRetriever.GetGamingGroupById(id);

            var model = new GamingGroupPublicDetailsViewModel
            {
                GamingGroupName = gamingGroup.Name,
                GamingGroupId = id,
                PublicDescription = gamingGroup.PublicDescription,
                Website = gamingGroup.PublicGamingGroupWebsite
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [UserContext]
        public virtual ActionResult Edit(GamingGroupEditRequest request, ApplicationUser currentUser)
        {
            if (ModelState.IsValid)
            {
                this.gamingGroupSaver.UpdatePublicGamingGroupDetails(request, currentUser);

                return RedirectToAction(MVC.GamingGroup.Details(currentUser.CurrentGamingGroupId, currentUser));
            }

            return (this.Edit(request.GamingGroupId));
        }
    }
}