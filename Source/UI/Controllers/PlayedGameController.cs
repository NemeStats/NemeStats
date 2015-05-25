﻿#region LICENSE
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

using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UI.Attributes.Filters;
using UI.Controllers.Helpers;
using UI.Models.PlayedGame;
using UI.Transformations;

namespace UI.Controllers
{
	public partial class PlayedGameController : Controller
	{
		internal NemeStatsDataContext dataContext;
		internal IPlayedGameRetriever playedGameRetriever;
		internal IPlayerRetriever playerRetriever;
		internal IPlayedGameDetailsViewModelBuilder playedGameDetailsBuilder;
		internal IPlayedGameCreator playedGameCreator;
		internal IGameDefinitionRetriever gameDefinitionRetriever;
		internal IShowingXResultsMessageBuilder showingXResultsMessageBuilder;
		internal IPlayedGameDeleter playedGameDeleter;

		internal const int NUMBER_OF_RECENT_GAMES_TO_DISPLAY = 25;

		public PlayedGameController(
			NemeStatsDataContext dataContext,
			IPlayedGameRetriever playedGameRetriever,
			IPlayerRetriever playerRetriever,
			IPlayedGameDetailsViewModelBuilder builder,
			IGameDefinitionRetriever gameDefinitionRetriever,
			IShowingXResultsMessageBuilder showingXResultsMessageBuilder,
			IPlayedGameCreator playedGameCreator,
			IPlayedGameDeleter playedGameDeleter)
		{
			this.dataContext = dataContext;
			this.playedGameRetriever = playedGameRetriever;
			this.playerRetriever = playerRetriever;
			this.playedGameDetailsBuilder = builder;
			this.gameDefinitionRetriever = gameDefinitionRetriever;
			this.showingXResultsMessageBuilder = showingXResultsMessageBuilder;
			this.playedGameCreator = playedGameCreator;
			this.playedGameDeleter = playedGameDeleter;
		}

		// GET: /PlayedGame/Details/5
		[UserContext(RequiresGamingGroup = false)]
		public virtual ActionResult Details(int? id, ApplicationUser currentUser)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			PlayedGame playedGame = playedGameRetriever.GetPlayedGameDetails(id.Value);
			if (playedGame == null)
			{
				return HttpNotFound();
			}
			PlayedGameDetailsViewModel playedGameDetails = playedGameDetailsBuilder.Build(playedGame, currentUser);
			return View(MVC.PlayedGame.Views.Details, playedGameDetails);
		}

		// GET: /PlayedGame/Create
		[Authorize]
		[UserContext]
		[HttpGet]
		public virtual ActionResult Create(ApplicationUser currentUser)
		{
			var viewModel = new PlayedGameEditViewModel
			{
				GameDefinitions = new SelectList(
					gameDefinitionRetriever.GetAllGameDefinitions(currentUser.CurrentGamingGroupId.Value),
					"Id",
					"Name"),
				Players = this.GetAllPlayers(currentUser)
			};

			return View(MVC.PlayedGame.Views.Create, viewModel);
		}

		[HttpGet]
		public virtual ActionResult ShowRecentlyPlayedGames()
		{
			var recentlyPlayedGames = playedGameRetriever.GetRecentPublicGames(NUMBER_OF_RECENT_GAMES_TO_DISPLAY);

			return View(MVC.PlayedGame.Views.RecentlyPlayedGames, recentlyPlayedGames);
		}

		// POST: /PlayedGame/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[UserContext]
		public virtual ActionResult Create(NewlyCompletedGame newlyCompletedGame, ApplicationUser currentUser)
		{
			if (ModelState.IsValid)
			{
				playedGameCreator.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, currentUser);

				return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
											+ "#" + GamingGroupController.SECTION_ANCHOR_RECENT_GAMES);
			}

			return Create(currentUser);
		}

		private IEnumerable<SelectListItem> GetAllPlayers(ApplicationUser currentUser)
		{
			List<Player> allPlayers = playerRetriever.GetAllPlayers(currentUser.CurrentGamingGroupId.Value);
			List<SelectListItem> allPlayersSelectList = allPlayers.Select(item => new SelectListItem
			{
				Text = item.Name,
				Value = item.Id.ToString()
			}).ToList();

			return allPlayersSelectList;
		}

		// GET: /PlayedGame/Delete/5
		[Authorize]
		[UserContext]
		public virtual ActionResult Delete(int? id, ApplicationUser currentUser)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			PlayedGame playedgame = dataContext.GetQueryable<PlayedGame>().FirstOrDefault(playedGame => playedGame.Id == id.Value);
			if (playedgame == null)
			{
				return HttpNotFound();
			}
			return View(playedgame);
		}

		// POST: /PlayedGame/Delete/5
		[Authorize]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[UserContext]
		public virtual ActionResult DeleteConfirmed(int id, ApplicationUser currentUser)
		{
			playedGameDeleter.DeletePlayedGame(id, currentUser);
			//TODO really don't know whether I need to commit here or if it is automatically taken care of when disposing.
			dataContext.CommitAllChanges();
			return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
							+ "#" + GamingGroupController.SECTION_ANCHOR_RECENT_GAMES);
		}

		// GET: /PlayedGame/Edit
		[Authorize]
		[UserContext]
		[HttpGet]
		public virtual ActionResult Edit(int id, ApplicationUser currentUser)
		{
			var viewModel = new PlayedGameEditViewModel();
			var gameDefinitionsList = this.gameDefinitionRetriever.GetAllGameDefinitions(currentUser.CurrentGamingGroupId.Value);
			viewModel.GameDefinitions = gameDefinitionsList.Select(item => new SelectListItem
			{
				Text = item.Name,
				Value = item.Id.ToString()
			}).ToList();
			viewModel.Players = this.GetAllPlayers(currentUser);

			if (id > 0)
			{
				var playedGame = this.playedGameRetriever.GetPlayedGameDetails(id);
				viewModel.PreviousGameId = playedGame.Id;
				viewModel.GameDefinitionId = playedGame.GameDefinitionId;
				viewModel.DatePlayed = playedGame.DatePlayed;
				viewModel.Notes = playedGame.Notes;

				viewModel.PlayerRanks = playedGame.PlayerGameResults.Select(item => new PlayerRank { GameRank = item.GameRank, PlayerId = item.PlayerId }).ToList();
				viewModel.ExistingRankedPlayerNames = playedGame.PlayerGameResults.Select(item => new { item.Player.Name, item.Player.Id }).ToDictionary(p => p.Name, q => q.Id);
				viewModel.Players = this.RemovePlayersFromExistingPlayerRanks(viewModel.Players.ToList(), viewModel.PlayerRanks);
			}

			return View(viewModel);
		}

		// POST: /PlayedGame/Edit
		[Authorize]
		[UserContext]
		[HttpPost]
		public virtual ActionResult Edit(NewlyCompletedGame newlyCompletedGame, int previousGameId, ApplicationUser currentUser)
		{
			if (ModelState.IsValid)
			{
				this.playedGameDeleter.DeletePlayedGame(previousGameId, currentUser);
                this.playedGameCreator.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, currentUser);

				return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
											+ "#" + GamingGroupController.SECTION_ANCHOR_RECENT_GAMES);
			}

			return RedirectToAction(MVC.PlayedGame.Edit(previousGameId, currentUser));
		}

		private IEnumerable<SelectListItem> RemovePlayersFromExistingPlayerRanks(IEnumerable<SelectListItem> players, List<PlayerRank> playerRanks)
		{
		    return players.Where(item => playerRanks.Any(p => p.PlayerId.ToString() == item.Value) == false).ToList();
		}

	    protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

        [Authorize]
        [UserContext]
        [HttpPost]
	    public ActionResult SearchPlayedGames(PlayedGamesFilterViewModel filter, ApplicationUser currentUser)
        {
            var viewModel = new SearchViewModel
            {
                Filter =
                {
                    DatePlayedEnd = filter.DatePlayedEnd,
                    DatePlayedStart = filter.DatePlayedStart,
                    GameDefinitionId = filter.GameDefinitionId
                }
            };

            return View(MVC.PlayedGame.Views.Search, viewModel);
	    }

        [Authorize]
        [UserContext]
        [HttpGet]
        public ActionResult SearchPlayedGames(ApplicationUser currentUser)
        {
            var viewModel = new SearchViewModel
            {
                GameDefinitions = GetAllGameDefinitionsForCurrentGamingGroup(currentUser)
            };
            return View(MVC.PlayedGame.Views.Search, viewModel);
	    }

        private IEnumerable<SelectListItem> GetAllGameDefinitionsForCurrentGamingGroup(ApplicationUser currentUser)
        {
            var gameDefinitions = gameDefinitionRetriever.GetAllGameDefinitionNames(currentUser);

            var selectListItems = gameDefinitions.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            selectListItems.Add(new SelectListItem
            {
                Selected = true,
                Text = "All",
                Value = string.Empty
            });

            return selectListItems;
        }
	}
}