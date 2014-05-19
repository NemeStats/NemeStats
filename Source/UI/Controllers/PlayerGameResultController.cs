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
    public class PlayerGameResultController : Controller
    {
        private NerdScorekeeperDbContext db = new NerdScorekeeperDbContext();

        // GET: /PlayerGameResult/
        public ActionResult Index()
        {
            var playergameresults = db.PlayerGameResults.Include(p => p.PlayedGame).Include(p => p.Player);
            return View(playergameresults.ToList());
        }

        // GET: /PlayerGameResult/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlayerGameResult playergameresult = db.PlayerGameResults.Find(id);
            if (playergameresult == null)
            {
                return HttpNotFound();
            }
            return View(playergameresult);
        }

        // GET: /PlayerGameResult/Create
        public ActionResult Create()
        {
            ViewBag.PlayedGameId = new SelectList(db.PlayedGames, "Id", "Id");
            ViewBag.PlayerId = new SelectList(db.Players, "Id", "Name");
            return View();
        }

        // POST: /PlayerGameResult/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,PlayedGameId,PlayerId")] PlayerGameResult playergameresult)
        {
            if (ModelState.IsValid)
            {
                db.PlayerGameResults.Add(playergameresult);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PlayedGameId = new SelectList(db.PlayedGames, "Id", "Id", playergameresult.PlayedGameId);
            ViewBag.PlayerId = new SelectList(db.Players, "Id", "Name", playergameresult.PlayerId);
            return View(playergameresult);
        }

        // GET: /PlayerGameResult/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlayerGameResult playergameresult = db.PlayerGameResults.Find(id);
            if (playergameresult == null)
            {
                return HttpNotFound();
            }
            ViewBag.PlayedGameId = new SelectList(db.PlayedGames, "Id", "Id", playergameresult.PlayedGameId);
            ViewBag.PlayerId = new SelectList(db.Players, "Id", "Name", playergameresult.PlayerId);
            return View(playergameresult);
        }

        // POST: /PlayerGameResult/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,PlayedGameId,PlayerId")] PlayerGameResult playergameresult)
        {
            if (ModelState.IsValid)
            {
                db.Entry(playergameresult).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PlayedGameId = new SelectList(db.PlayedGames, "Id", "Id", playergameresult.PlayedGameId);
            ViewBag.PlayerId = new SelectList(db.Players, "Id", "Name", playergameresult.PlayerId);
            return View(playergameresult);
        }

        // GET: /PlayerGameResult/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlayerGameResult playergameresult = db.PlayerGameResults.Find(id);
            if (playergameresult == null)
            {
                return HttpNotFound();
            }
            return View(playergameresult);
        }

        // POST: /PlayerGameResult/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PlayerGameResult playergameresult = db.PlayerGameResults.Find(id);
            db.PlayerGameResults.Remove(playergameresult);
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
