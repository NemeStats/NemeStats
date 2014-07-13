using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UI.Filters;
using UI.Models.PlayedGame;
using UI.Models.Players;
using UI.Transformations;
using UI.Transformations.Player;

namespace UI.Controllers
{
    [Authorize]
    public partial class PlayerController : Controller
    {
        public static readonly int NUMBER_OF_RECENT_GAMES_TO_RETRIEVE = 10;
        public static readonly string RECENT_GAMES_MESSAGE_FORMAT = "(Last {0} Games)";


        internal NemeStatsDbContext db;
        internal PlayerRepository playerRepository;
        internal GameResultViewModelBuilder builder;
        internal PlayerDetailsViewModelBuilder playerDetailsViewModelBuilder;
        
        public PlayerController(NemeStatsDbContext dbContext, 
            PlayerRepository playerRepository, 
            GameResultViewModelBuilder resultBuilder,
            PlayerDetailsViewModelBuilder playerDetailsBuilder)
        {
            db = dbContext;
            this.playerRepository = playerRepository;
            builder = resultBuilder;
            playerDetailsViewModelBuilder = playerDetailsBuilder;
        }

        // GET: /Player/
        [UserContextActionFilter]
        public virtual ActionResult Index(UserContext userContext)
        {
            return View(MVC.Player.Views.Index, playerRepository.GetAllPlayers(true, userContext));
        }

        // GET: /Player/Details/5
        [UserContextActionFilter]
        public virtual ActionResult Details(int? id, UserContext userContext)
        {
            if(!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PlayerDetails player = playerRepository.GetPlayerDetails(id.Value, NUMBER_OF_RECENT_GAMES_TO_RETRIEVE, userContext);

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
        public virtual ActionResult Create()
        {
            return View();
        }

        // POST: /Player/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserContextActionFilter]
        public virtual ActionResult Create([Bind(Include = "Id,Name,Active")] Player player, UserContext userContext)
        {
            if (ModelState.IsValid)
            {
                playerRepository.Save(player, userContext);
                return RedirectToAction(MVC.Player.ActionNames.Index);
            }

            return View(MVC.Player.Views.Index, player);
        }

        // GET: /Player/Edit/5
        [UserContextActionFilter]
        public virtual ActionResult Edit(int? id, UserContext userContext)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlayerDetails player;
            try
            {
                player = playerRepository.GetPlayerDetails(id.Value, 0, userContext);
            }catch(UnauthorizedAccessException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }catch(KeyNotFoundException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(MVC.Player.Views.Edit, player);
        }

        // POST: /Player/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserContextActionFilter]
        public virtual ActionResult Edit([Bind(Include = "Id,Name,Active")] Player player, UserContext userContext)
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
        [UserContextActionFilter]
        public virtual ActionResult Delete(int? id, UserContext userContext)
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
        [UserContextActionFilter]
        public virtual ActionResult DeleteConfirmed(int id, UserContext userContext)
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
