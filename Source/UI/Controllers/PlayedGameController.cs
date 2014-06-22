using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UI.Models.PlayedGame;
using UI.Transformations;

namespace UI.Controllers
{
    public partial class PlayedGameController : Controller
    {
        internal NemeStatsDbContext db;
        internal PlayedGameLogic playedGameLogic;
        internal PlayerLogic playerLogic;
        internal PlayedGameDetailsViewModelBuilder playedGameDetailsBuilder;

        internal const int NUMBER_OF_RECENT_GAMES_TO_DISPLAY = 10;

        public PlayedGameController(NemeStatsDbContext dbContext, 
            PlayedGameLogic playedLogic, 
            PlayerLogic playLogic,
            PlayedGameDetailsViewModelBuilder builder)
        {
            db = dbContext;
            playedGameLogic = playedLogic;
            playerLogic = playLogic;
            playedGameDetailsBuilder = builder;
        }

        // GET: /PlayedGame/
        public virtual ActionResult Index()
        {
            List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(NUMBER_OF_RECENT_GAMES_TO_DISPLAY);
            int totalGames = playedGames.Count();
            List<PlayedGameDetailsViewModel> details = new List<PlayedGameDetailsViewModel>(totalGames);
            for (int i = 0; i < totalGames; i++)
            {
                details.Add(playedGameDetailsBuilder.Build(playedGames[i]));
            }
            
            return View(MVC.PlayedGame.Views.Index, details);
        }

        // GET: /PlayedGame/Details/5
        public virtual ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlayedGame playedGame = playedGameLogic.GetPlayedGameDetails(id.Value);
            if (playedGame == null)
            {
                return HttpNotFound();
            }
            PlayedGameDetailsViewModel playedGameDetails = playedGameDetailsBuilder.Build(playedGame);
            return View(MVC.PlayedGame.Views.Details, playedGameDetails);
        }

        // GET: /PlayedGame/Create
        public virtual ActionResult Create()
        {
            ViewBag.GameDefinitionId = new SelectList(db.GameDefinitions, "Id", "Name");

            AddAllPlayersToViewBag();

            return View(MVC.PlayedGame.Views.Create);
        }

        private void AddAllPlayersToViewBag()
        {
            //TODO Clean Code said something about boolean parameters not being good. Come back to this...
            List<Player> allPlayers = playerLogic.GetAllPlayers(true);
            List<SelectListItem> allPlayersSelectList = allPlayers.Select(item => new SelectListItem()
            {
                Text = item.Name,
                Value = item.Id.ToString()
            }).ToList();

            ViewBag.Players = allPlayersSelectList;
        }

        // POST: /PlayedGame/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create(NewlyCompletedGame newlyCompletedGame)
        {
            if (ModelState.IsValid)
            {
                playedGameLogic.CreatePlayedGame(newlyCompletedGame);

                return RedirectToAction(MVC.PlayedGame.ActionNames.Index);
            }

            AddAllPlayersToViewBag();

            ViewBag.GameDefinitionId = new SelectList(db.GameDefinitions, "Id", "Name", newlyCompletedGame.GameDefinitionId);
            //TODO If validation fails, should not lose all of the players that have been added already.
            return View(MVC.PlayedGame.Views.Create, newlyCompletedGame);
        }


        // GET: /PlayedGame/Edit/5
        public virtual ActionResult Edit(int? id)
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
        public virtual ActionResult Edit([Bind(Include = "Id,GameDefinitionId,NumberOfPlayers")] PlayedGame playedgame)
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
        public virtual ActionResult Delete(int? id)
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
        public virtual ActionResult DeleteConfirmed(int id)
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
