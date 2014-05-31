using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BusinessLogic.Models;
using BusinessLogic.DataAccess;

namespace UI.Controllers
{
    public partial class PlayedGameController : Controller
    {
        private NerdScorekeeperDbContext db = new NerdScorekeeperDbContext();

        // GET: /PlayedGame/
        public virtual ActionResult Index()
        {
            var playedgames = db.PlayedGames.Include(p => p.GameDefinition);
            return View(playedgames.ToList());
        }

        // GET: /PlayedGame/Details/5
        public virtual ActionResult Details(int? id)
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

        // GET: /PlayedGame/Create
        public virtual ActionResult Create()
        {
            ViewBag.GameDefinitionId = new SelectList(db.GameDefinitions, "Id", "Name");
            return View();
        }

        // POST: /PlayedGame/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create([Bind(Include = "Id,GameDefinitionId,NumberOfPlayers")] PlayedGame playedgame)
        {
            if (ModelState.IsValid)
            {
                db.PlayedGames.Add(playedgame);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GameDefinitionId = new SelectList(db.GameDefinitions, "Id", "Name", playedgame.GameDefinitionId);
            return View(playedgame);
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
