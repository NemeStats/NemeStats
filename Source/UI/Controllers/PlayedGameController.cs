using BusinessLogic.DataAccess;
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
using UI.Controllers.Helpers;
using UI.Filters;
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
		[UserContextAttribute(RequiresGamingGroup = false)]
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
		[UserContextAttribute]
		[HttpGet]
		public virtual ActionResult Create(ApplicationUser currentUser)
		{
			var viewModel = new NewlyCompletedGameViewModel
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
		[UserContextAttribute]
		public virtual ActionResult Create(NewlyCompletedGame newlyCompletedGame, ApplicationUser currentUser)
		{
			if (ModelState.IsValid)
			{
				playedGameCreator.CreatePlayedGame(newlyCompletedGame, currentUser);

				return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
											+ "#" + GamingGroupController.SECTION_ANCHOR_RECENT_GAMES);
			}

			return Create(currentUser);
		}

		private IEnumerable<SelectListItem> GetAllPlayers(ApplicationUser currentUser)
		{
			List<Player> allPlayers = playerRetriever.GetAllPlayers(currentUser.CurrentGamingGroupId.Value);
			List<SelectListItem> allPlayersSelectList = allPlayers.Select(item => new SelectListItem()
			{
				Text = item.Name,
				Value = item.Id.ToString()
			}).ToList();

			return allPlayersSelectList;
		}

		// GET: /PlayedGame/Delete/5
		[Authorize]
		[UserContextAttribute]
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
		[UserContextAttribute]
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
		[UserContextAttribute]
		[HttpGet]
		public virtual ActionResult Edit(int? id, ApplicationUser currentUser)
		{
			var viewModel = new NewlyCompletedGameViewModel();
			viewModel.GameDefinitions = this.gameDefinitionRetriever.GetAllGameDefinitions(currentUser.CurrentGamingGroupId.Value).Select(item => new SelectListItem { Text = item.Name, Value = item.Id.ToString() }).ToList();
			viewModel.Players = this.GetAllPlayers(currentUser);

			if (id > 0)
			{
				var playedGame = this.playedGameRetriever.GetPlayedGameDetails((int)id) as PlayedGame;
				viewModel.GameDefinitionId = playedGame.GameDefinitionId;
				viewModel.DatePlayed = playedGame.DatePlayed;
				viewModel.Notes = playedGame.Notes;

				//need to send player ranks to view
				//probably needs to be saved on recorded game creation
			}

			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			return View(viewModel);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}