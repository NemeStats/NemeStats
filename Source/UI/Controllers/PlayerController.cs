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
using UI.Controllers.Helpers;
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

        internal DataContext dataContext;
        internal PlayerRepository playerRepository;
        internal GameResultViewModelBuilder builder;
        internal PlayerDetailsViewModelBuilder playerDetailsViewModelBuilder;
        internal ShowingXResultsMessageBuilder showingXResultsMessageBuilder;
        
        public PlayerController(DataContext dataContext, 
            PlayerRepository playerRepository, 
            GameResultViewModelBuilder builder,
            PlayerDetailsViewModelBuilder playerDetailsViewModelBuilder,
            ShowingXResultsMessageBuilder showingXResultsMessageBuilder)
        {
            this.dataContext = dataContext;
            this.playerRepository = playerRepository;
            this.builder = builder;
            this.playerDetailsViewModelBuilder = playerDetailsViewModelBuilder;
            this.showingXResultsMessageBuilder = showingXResultsMessageBuilder;
        }

        // GET: /Player/
        [UserContextAttribute]
        public virtual ActionResult Index(ApplicationUser currentUser)
        {
            List<Player> players = playerRepository.GetAllPlayers(true, currentUser);
            return View(MVC.Player.Views.Index, players);
        }

        // GET: /Player/Details/5
        [UserContextAttribute]
        public virtual ActionResult Details(int? id, ApplicationUser currentUser)
        {
            if(!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PlayerDetails player = playerRepository.GetPlayerDetails(id.Value, NUMBER_OF_RECENT_GAMES_TO_RETRIEVE, currentUser);

            if (player == null)
            {
                return new HttpNotFoundResult();
            }

            PlayerDetailsViewModel playerDetailsViewModel = playerDetailsViewModelBuilder.Build(player);

            ViewBag.RecentGamesMessage = showingXResultsMessageBuilder.BuildMessage(
                NUMBER_OF_RECENT_GAMES_TO_RETRIEVE, 
                player.PlayerGameResults.Count);

            return View(MVC.Player.Views.Details, playerDetailsViewModel);
        }

        // GET: /Player/Create
        public virtual ActionResult Create()
        {
            return View(MVC.Player.Views.Create, new Player());
        }

        // POST: /Player/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserContextAttribute]
        public virtual ActionResult Create([Bind(Include = "Id,Name,Active")] Player player, ApplicationUser currentUser)
        {
            if (ModelState.IsValid)
            {
                dataContext.Save<Player>(player, currentUser);
                dataContext.CommitAllChanges();
                return RedirectToAction(MVC.Player.ActionNames.Index);
            }

            return View(MVC.Player.Views.Index, player);
        }

        // GET: /Player/Edit/5
        [UserContextAttribute]
        public virtual ActionResult Edit(int? id, ApplicationUser currentUser)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlayerDetails player;
            try
            {
                player = playerRepository.GetPlayerDetails(id.Value, 0, currentUser);
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
        [UserContextAttribute]
        public virtual ActionResult Edit([Bind(Include = "Id,Name,Active,GamingGroupId")] Player player, ApplicationUser currentUser)
        {
            if (ModelState.IsValid)
            {
                dataContext.Save<Player>(player, currentUser);
                dataContext.CommitAllChanges();
                return RedirectToAction(MVC.Player.ActionNames.Index);
            }
            return View(MVC.Player.Views.Edit, player);
        }

        // GET: /Player/Delete/5
        [UserContextAttribute]
        public virtual ActionResult Delete(int? id, ApplicationUser currentUser)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlayerDetails playerDetails;
            try
            {
                playerDetails = playerRepository.GetPlayerDetails(id.Value, 0, currentUser);
            }catch(UnauthorizedAccessException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }catch (KeyNotFoundException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            
            return View(MVC.Player.Views.Delete, playerDetails);
        }

        // POST: /Player/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [UserContextAttribute]
        public virtual ActionResult DeleteConfirmed(int id, ApplicationUser currentUser)
        {
            try
            {
                dataContext.DeleteById<Player>(id, currentUser);
                dataContext.CommitAllChanges();
            }catch(UnauthorizedAccessException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
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
