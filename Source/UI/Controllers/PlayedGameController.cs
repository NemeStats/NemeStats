using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using System.Collections.Generic;
using System.Data.Entity;
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
        internal PlayedGameRetriever playedGameRetriever;
        internal PlayerRetriever playerRetriever;
        internal PlayedGameDetailsViewModelBuilder playedGameDetailsBuilder;
        internal PlayedGameCreator playedGameCreator;
        internal GameDefinitionRetriever gameDefinitionRetriever;
        internal ShowingXResultsMessageBuilder showingXResultsMessageBuilder;

        internal const int NUMBER_OF_RECENT_GAMES_TO_DISPLAY = 10;

        public PlayedGameController(
            NemeStatsDataContext dataContext,
            PlayedGameRetriever playedGameRetriever, 
            PlayerRetriever playerRetriever,
            PlayedGameDetailsViewModelBuilder builder,
            GameDefinitionRetriever gameDefinitionRetriever,
            ShowingXResultsMessageBuilder showingXResultsMessageBuilder,
            PlayedGameCreator playedGameCreator)
        {
            this.dataContext = dataContext;
            this.playedGameRetriever = playedGameRetriever;
            this.playerRetriever = playerRetriever;
            playedGameDetailsBuilder = builder;
            this.gameDefinitionRetriever = gameDefinitionRetriever;
            this.showingXResultsMessageBuilder = showingXResultsMessageBuilder;
            this.playedGameCreator = playedGameCreator;
        }

        // GET: /PlayedGame/
        [Authorize]
        [UserContextAttribute]
        public virtual ActionResult Index(ApplicationUser currentUser)
        {
            List<PlayedGame> playedGames = playedGameRetriever.GetRecentGames(
                NUMBER_OF_RECENT_GAMES_TO_DISPLAY, 
                currentUser.CurrentGamingGroupId.Value);
            int totalGames = playedGames.Count();
            List<PlayedGameDetailsViewModel> details = new List<PlayedGameDetailsViewModel>(totalGames);
            for (int i = 0; i < totalGames; i++)
            {
                details.Add(playedGameDetailsBuilder.Build(playedGames[i], currentUser));
            }

            ViewBag.RecentGamesMessage = showingXResultsMessageBuilder.BuildMessage(
                NUMBER_OF_RECENT_GAMES_TO_DISPLAY,
                details.Count);
            
            return View(MVC.PlayedGame.Views.Index, details);
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
            ViewBag.GameDefinitionId = new SelectList(
                gameDefinitionRetriever.GetAllGameDefinitions(currentUser.CurrentGamingGroupId.Value), 
                "Id", 
                "Name");

            AddAllPlayersToViewBag(currentUser);

            return View(MVC.PlayedGame.Views.Create);
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

                return RedirectToAction(MVC.PlayedGame.ActionNames.Index);
            }

            return Create(currentUser);
        }

        private void AddAllPlayersToViewBag(ApplicationUser currentUser)
        {
            //TODO Clean Code said something about boolean parameters not being good. Come back to this...
            List<Player> allPlayers = playerRetriever.GetAllPlayers(currentUser.CurrentGamingGroupId.Value);
            List<SelectListItem> allPlayersSelectList = allPlayers.Select(item => new SelectListItem()
            {
                Text = item.Name,
                Value = item.Id.ToString()
            }).ToList();

            ViewBag.Players = allPlayersSelectList;
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
            PlayedGame playedgame = dataContext.GetQueryable<PlayedGame>()
                .Where(playedGame => playedGame.Id == id.Value)
                .FirstOrDefault();
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
            dataContext.DeleteById<PlayedGame>(id, currentUser);
            dataContext.CommitAllChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dataContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
