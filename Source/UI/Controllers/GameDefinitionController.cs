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
        internal NemeStatsDbContext nemeStatsDbContext;
        internal ApplicationDataContext applicationDataContext;
        internal GameDefinitionRepository gameDefinitionRepository;

        public GameDefinitionController(NemeStatsDbContext db, ApplicationDataContext dbContext, GameDefinitionRepository gameDefinitionRepository)
        {
            this.nemeStatsDbContext = db;
            this.gameDefinitionRepository = gameDefinitionRepository;
            this.applicationDataContext = dbContext;
        }

        // GET: /GameDefinition/
        [UserContextAttribute]
        public virtual ActionResult Index(ApplicationUser currentUser)
        {
            List<GameDefinition> games = gameDefinitionRepository.GetAllGameDefinitions(currentUser);
            return View(MVC.GameDefinition.Views.Index, games);
        }

        // GET: /GameDefinition/Details/5
        [UserContextAttribute]
        public virtual ActionResult Details(int? id, ApplicationUser currentUser)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GameDefinition gameDefinition;

            try
            {
                gameDefinition = gameDefinitionRepository.GetGameDefinition(id.Value, currentUser);
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
            return View(MVC.GameDefinition.Views.Create);
        }

        // POST: /GameDefinition/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserContextAttribute]
        public virtual ActionResult Create([Bind(Include = "Id,Name,Description")] GameDefinition gameDefinition, ApplicationUser currentUser)
        {
            if (ModelState.IsValid)
            {
                applicationDataContext.Save(gameDefinition, currentUser);
                return RedirectToAction(MVC.GameDefinition.ActionNames.Index);
            }

            return View(MVC.GameDefinition.Views.Create, gameDefinition);
        }

        // GET: /GameDefinition/Edit/5
        [UserContextAttribute]
        public virtual ActionResult Edit(int? id, ApplicationUser currentUser)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GameDefinition gameDefinition;
            try
            {
                gameDefinition = gameDefinitionRepository.GetGameDefinition(id.Value, currentUser);
            }catch(KeyNotFoundException)
            {
                return HttpNotFound();
            }

            return View(MVC.GameDefinition.Views.Edit, gameDefinition);
        }

        // POST: /GameDefinition/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserContextAttribute]
        public virtual ActionResult Edit([Bind(Include = "Id,Name,Description,GamingGroupId")] GameDefinition gamedefinition, ApplicationUser currentUser)
        {
            if (ModelState.IsValid)
            {
                applicationDataContext.Save(gamedefinition, currentUser);
                return RedirectToAction(MVC.GameDefinition.ActionNames.Index);
            }
            return View(MVC.GameDefinition.Views.Edit, gamedefinition);
        }

        // GET: /GameDefinition/Delete/5
        [UserContextAttribute]
        public virtual ActionResult Delete(int? id, ApplicationUser currentUser)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                GameDefinition gameDefinition = gameDefinitionRepository.GetGameDefinition(id.Value, currentUser);
                return View(MVC.GameDefinition.Views.Delete, gameDefinition);
            }catch(KeyNotFoundException)
            {
                return HttpNotFound();
            }catch (UnauthorizedAccessException)
            {
                return new HttpUnauthorizedResult();
            }
        }

        // POST: /GameDefinition/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [UserContextAttribute]
        public virtual ActionResult DeleteConfirmed(int id, ApplicationUser currentUser)
        {
            try
            {
                gameDefinitionRepository.Delete(id, currentUser);
                return RedirectToAction(MVC.GameDefinition.ActionNames.Index);
            }catch(UnauthorizedAccessException)
            {
                return new HttpUnauthorizedResult();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                applicationDataContext.Dispose();
                nemeStatsDbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
