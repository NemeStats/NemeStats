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
using BusinessLogic.Logic.GameDefinitions;
using UI.Transformations;
using UI.Models.GameDefinitionModels;
using UI.Controllers.Helpers;

namespace UI.Controllers
{
    [Authorize]
    public partial class GameDefinitionController : Controller
    {
        internal const int NUMBER_OF_RECENT_GAMES_TO_SHOW = 5;

        internal DataContext dataContext;
        internal GameDefinitionRepository gameDefinitionRepository;
        internal GameDefinitionRetriever gameDefinitionRetriever;
        internal GameDefinitionToGameDefinitionViewModelTransformation gameDefinitionTransformation;
        internal ShowingXResultsMessageBuilder showingXResultsMessageBuilder;

        public GameDefinitionController(DataContext dataContext, 
            GameDefinitionRepository gameDefinitionRepository, 
            GameDefinitionRetriever gameDefinitionRetriever,
            GameDefinitionToGameDefinitionViewModelTransformation gameDefinitionTransformation,
            ShowingXResultsMessageBuilder showingXResultsMessageBuilder)
        {
            this.gameDefinitionRepository = gameDefinitionRepository;
            this.dataContext = dataContext;
            this.gameDefinitionRetriever = gameDefinitionRetriever;
            this.gameDefinitionTransformation = gameDefinitionTransformation;
            this.showingXResultsMessageBuilder = showingXResultsMessageBuilder;
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
            GameDefinitionViewModel gameDefinitionViewModel;

            try
            {
                gameDefinition = gameDefinitionRetriever.GetGameDefinitionDetails(id.Value, NUMBER_OF_RECENT_GAMES_TO_SHOW, currentUser);
                gameDefinitionViewModel = gameDefinitionTransformation.Build(gameDefinition);
            }catch(KeyNotFoundException)
            {
                return new HttpNotFoundResult(); ;
            }
            catch (UnauthorizedAccessException)
            {
                return new HttpUnauthorizedResult();
            }

            ViewBag.RecentGamesMessage = showingXResultsMessageBuilder.BuildMessage(NUMBER_OF_RECENT_GAMES_TO_SHOW, gameDefinition.PlayedGames.Count);

            return View(MVC.GameDefinition.Views.Details, gameDefinitionViewModel);
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
                dataContext.Save(gameDefinition, currentUser);
                dataContext.CommitAllChanges();
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
                gameDefinition = gameDefinitionRetriever.GetGameDefinitionDetails(id.Value, 0, currentUser);
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
                dataContext.Save(gamedefinition, currentUser);
                dataContext.CommitAllChanges();
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
                GameDefinition gameDefinition = gameDefinitionRetriever.GetGameDefinitionDetails(
                    id.Value, 
                    0, 
                    currentUser);
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
                dataContext.DeleteById<GameDefinition>(id, currentUser);
                dataContext.CommitAllChanges();
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
                dataContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
