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
    public class GameDefinitionController : Controller
    {
        private NerdScorekeeperDbContext db = new NerdScorekeeperDbContext();

        // GET: /GameDefinition/
        public ActionResult Index()
        {
            return View(db.GameDefinitions.ToList());
        }

        // GET: /GameDefinition/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GameDefinition gamedefinition = db.GameDefinitions.Find(id);
            if (gamedefinition == null)
            {
                return HttpNotFound();
            }
            return View(gamedefinition);
        }

        // GET: /GameDefinition/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /GameDefinition/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Name,Description")] GameDefinition gamedefinition)
        {
            if (ModelState.IsValid)
            {
                db.GameDefinitions.Add(gamedefinition);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(gamedefinition);
        }

        // GET: /GameDefinition/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GameDefinition gamedefinition = db.GameDefinitions.Find(id);
            if (gamedefinition == null)
            {
                return HttpNotFound();
            }
            return View(gamedefinition);
        }

        // POST: /GameDefinition/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Name,Description")] GameDefinition gamedefinition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gamedefinition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gamedefinition);
        }

        // GET: /GameDefinition/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GameDefinition gamedefinition = db.GameDefinitions.Find(id);
            if (gamedefinition == null)
            {
                return HttpNotFound();
            }
            return View(gamedefinition);
        }

        // POST: /GameDefinition/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GameDefinition gamedefinition = db.GameDefinitions.Find(id);
            db.GameDefinitions.Remove(gamedefinition);
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
