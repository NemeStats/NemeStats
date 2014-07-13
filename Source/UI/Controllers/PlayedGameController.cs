using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UI.Filters;
using UI.Models.PlayedGame;
using UI.Transformations;

namespace UI.Controllers
{
    [Authorize]
    public partial class PlayedGameController : Controller
    {
        internal NemeStatsDbContext db;
        internal PlayedGameLogic playedGameLogic;
        internal PlayerRepository playerLogic;
        internal PlayedGameDetailsViewModelBuilder playedGameDetailsBuilder;

        internal const int NUMBER_OF_RECENT_GAMES_TO_DISPLAY = 10;

        public PlayedGameController(NemeStatsDbContext dbContext, 
            PlayedGameLogic playedLogic, 
            PlayerRepository playLogic,
            PlayedGameDetailsViewModelBuilder builder)
        {
            db = dbContext;
            playedGameLogic = playedLogic;
            playerLogic = playLogic;
            playedGameDetailsBuilder = builder;
        }

        // GET: /PlayedGame/
        [UserContextActionFilter]
        public virtual ActionResult Index(UserContext userContext)
        {
            List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(NUMBER_OF_RECENT_GAMES_TO_DISPLAY, userContext);
            int totalGames = playedGames.Count();
            List<PlayedGameDetailsViewModel> details = new List<PlayedGameDetailsViewModel>(totalGames);
            for (int i = 0; i < totalGames; i++)
            {
                details.Add(playedGameDetailsBuilder.Build(playedGames[i]));
            }
            
            return View(MVC.PlayedGame.Views.Index, details);
        }

        // GET: /PlayedGame/Details/5
        [UserContextActionFilter]
        public virtual ActionResult Details(int? id, UserContext userContext)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlayedGame playedGame = playedGameLogic.GetPlayedGameDetails(id.Value, userContext);
            if (playedGame == null)
            {
                return HttpNotFound();
            }
            PlayedGameDetailsViewModel playedGameDetails = playedGameDetailsBuilder.Build(playedGame);
            return View(MVC.PlayedGame.Views.Details, playedGameDetails);
        }

        // GET: /PlayedGame/Create
        [UserContextActionFilter]
        public virtual ActionResult Create(UserContext userContext)
        {
            ViewBag.GameDefinitionId = new SelectList(db.GameDefinitions, "Id", "Name");

            AddAllPlayersToViewBag(userContext);

            return View(MVC.PlayedGame.Views.Create);
        }

        // POST: /PlayedGame/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserContextActionFilter]
        public virtual ActionResult Create(NewlyCompletedGame newlyCompletedGame, UserContext userContext)
        {
            if (ModelState.IsValid)
            {
                playedGameLogic.CreatePlayedGame(newlyCompletedGame, userContext);

                return RedirectToAction(MVC.PlayedGame.ActionNames.Index);
            }

            return Create(userContext);
        }

        private void AddAllPlayersToViewBag(UserContext requestingUserContext)
        {
            //TODO Clean Code said something about boolean parameters not being good. Come back to this...
            List<Player> allPlayers = playerLogic.GetAllPlayers(true, requestingUserContext);
            List<SelectListItem> allPlayersSelectList = allPlayers.Select(item => new SelectListItem()
            {
                Text = item.Name,
                Value = item.Id.ToString()
            }).ToList();

            ViewBag.Players = allPlayersSelectList;
        }

        // GET: /PlayedGame/Edit/5
        [UserContextActionFilter]
        public virtual ActionResult Edit(int? id, UserContext userContext)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlayedGame playedgame = db.PlayedGames.Find(id);
            if (playedgame == null)
            {
                return HttpNotFound();
            }
            ViewBag.GameDefinitionId = new SelectList(db.GameDefinitions, "Id", "Name", playedgame.GameDefinitionId);
            return View(playedgame);
        }

        // POST: /PlayedGame/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserContextActionFilter]
        public virtual ActionResult Edit([Bind(Include = "Id,GameDefinitionId,NumberOfPlayers")] PlayedGame playedgame, UserContext userContext)
        {
            if (ModelState.IsValid)
            {
                db.Entry(playedgame).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GameDefinitionId = new SelectList(db.GameDefinitions, "Id", "Name", playedgame.GameDefinitionId);
            return View(playedgame);
        }

        // GET: /PlayedGame/Delete/5
        [UserContextActionFilter]
        public virtual ActionResult Delete(int? id, UserContext userContext)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlayedGame playedgame = db.PlayedGames.Find(id);
            if (playedgame == null)
            {
                return HttpNotFound();
            }
            return View(playedgame);
        }

        // POST: /PlayedGame/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [UserContextActionFilter]
        public virtual ActionResult DeleteConfirmed(int id, UserContext userContext)
        {
            PlayedGame playedgame = db.PlayedGames.Find(id);
            db.PlayedGames.Remove(playedgame);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
