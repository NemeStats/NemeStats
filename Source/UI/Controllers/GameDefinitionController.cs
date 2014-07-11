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
using BusinessLogic.Models.User;
using BusinessLogic.Logic;
using UI.Filters;
using BusinessLogic.DataAccess.Repositories;

namespace UI.Controllers
{
    [Authorize]
    public partial class GameDefinitionController : Controller
    {
        internal NemeStatsDbContext db;
        internal GameDefinitionRepository gameDefinitionRepository;

        public GameDefinitionController(NemeStatsDbContext dbContext, GameDefinitionRepository gameDefinitionRepository)
        {
            this.db = dbContext;
            this.gameDefinitionRepository = gameDefinitionRepository;
        }

        // GET: /GameDefinition/
        [UserContextActionFilter]
        public virtual ActionResult Index(UserContext userContext)
        {
            List<GameDefinition> games = gameDefinitionRepository.GetAllGameDefinitions(db, userContext);
            return View(MVC.GameDefinition.Views.Index, games);
        }

        // GET: /GameDefinition/Details/5
        [UserContextActionFilter]
        public virtual ActionResult Details(int? id, UserContext userContext)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GameDefinition gameDefinition;

            try
            {
                gameDefinition = gameDefinitionRepository.GetGameDefinition(id.Value, db, userContext);
            }catch(KeyNotFoundException)
            {
                return new HttpNotFoundResult(); ;
            }
            catch (UnauthorizedAccessException)
            {
                return new HttpUnauthorizedResult();
            }

            return View(MVC.GameDefinition.Views.Details, gameDefinition);
        }

        // GET: /GameDefinition/Create
        public virtual ActionResult Create()
        {
            return View();
        }

        // POST: /GameDefinition/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserContextActionFilter]
        public virtual ActionResult Create([Bind(Include = "Id,Name,Description")] GameDefinition gameDefinition, UserContext userContext)
        {
            if (ModelState.IsValid)
            {
                gameDefinition.GamingGroupId = userContext.GamingGroupId;
                db.GameDefinitions.Add(gameDefinition);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(gameDefinition);
        }

        // GET: /GameDefinition/Edit/5
        [UserContextActionFilter]
        public virtual ActionResult Edit(int? id, UserContext userContext)
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
        [UserContextActionFilter]
        public virtual ActionResult Edit([Bind(Include = "Id,Name,Description")] GameDefinition gamedefinition, UserContext userContext)
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
        [UserContextActionFilter]
        public virtual ActionResult Delete(int? id, UserContext userContext)
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
        [UserContextActionFilter]
        public virtual ActionResult DeleteConfirmed(int id, UserContext userContext)
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
