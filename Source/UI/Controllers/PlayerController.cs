using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UI.Models.PlayedGame;
using UI.Models.Players;
using UI.Transformations;
using UI.Transformations.Player;

namespace UI.Controllers
{
    public class PlayerController : Controller
    {
        public static readonly int NUMBER_OF_RECENT_GAMES_TO_RETRIEVE = 10;
        public static readonly string RECENT_GAMES_MESSAGE_FORMAT = "(Last {0} Games)";


        internal NemeStatsDbContext db;
        internal PlayerLogic playerLogic;
        internal GameResultViewModelBuilder builder;
        internal PlayerDetailsViewModelBuilder playerDetailsViewModelBuilder;
        
        public PlayerController(NemeStatsDbContext dbContext, 
            PlayerLogic logic, 
            GameResultViewModelBuilder resultBuilder,
            PlayerDetailsViewModelBuilder playerDetailsBuilder)
        {
            db = dbContext;
            playerLogic = logic;
            builder = resultBuilder;
            playerDetailsViewModelBuilder = playerDetailsBuilder;
        }

        // GET: /Player/
        public ActionResult Index()
        {
            return View(db.Players.ToList());
        }

        // GET: /Player/Details/5
        public ActionResult Details(int? id)
        {
            if(!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PlayerDetails player = playerLogic.GetPlayerDetails(id.Value, NUMBER_OF_RECENT_GAMES_TO_RETRIEVE);

            if (player == null)
            {
                return new HttpNotFoundResult();
            }

            PlayerDetailsViewModel playerDetailsViewModel = playerDetailsViewModelBuilder.Build(player);

            int numberOfRecentGames = player.PlayerGameResults.Count;
            if(numberOfRecentGames >= NUMBER_OF_RECENT_GAMES_TO_RETRIEVE)
            {
                ViewBag.RecentGamesMessage = string.Format(RECENT_GAMES_MESSAGE_FORMAT, player.PlayerGameResults.Count);
            }
            
            return View(MVC.Player.Views.Details, playerDetailsViewModel);
        }

        // GET: /Player/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Player/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Name,Active")] Player player)
        {
            if (ModelState.IsValid)
            {
                db.Players.Add(player);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(player);
        }

        // GET: /Player/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // POST: /Player/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Name,Active")] Player player)
        {
            if (ModelState.IsValid)
            {
                db.Entry(player).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(player);
        }

        // GET: /Player/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // POST: /Player/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Player player = db.Players.Find(id);
            db.Players.Remove(player);
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
