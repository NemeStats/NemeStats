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
        internal IPlayedGameRetriever playedGameRetriever;
        internal IPlayerRetriever playerRetriever;
        internal IPlayedGameDetailsViewModelBuilder playedGameDetailsBuilder;
        internal IPlayedGameCreator playedGameCreator;
        internal IGameDefinitionRetriever gameDefinitionRetriever;
        internal IShowingXResultsMessageBuilder showingXResultsMessageBuilder;
        internal IPlayedGameDeleter playedGameDeleter;

        internal const int NUMBER_OF_RECENT_GAMES_TO_DISPLAY = 10;

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
            playedGameDetailsBuilder = builder;
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

                return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
                                            + "#" + GamingGroupController.SECTION_ANCHOR_RECENT_GAMES);
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
            playedGameDeleter.DeletePlayedGame(id, currentUser);
            //TODO really don't know whether I need to commit here or if it is automatically taken care of when disposing.
            dataContext.CommitAllChanges();
            return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
                            + "#" + GamingGroupController.SECTION_ANCHOR_RECENT_GAMES);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
