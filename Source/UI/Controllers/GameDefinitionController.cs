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
    public partial class GameDefinitionController : Controller
    {
        internal const int NUMBER_OF_RECENT_GAMES_TO_SHOW = 5;

        internal DataContext dataContext;
        internal GameDefinitionRetriever gameDefinitionRetriever;
        internal GameDefinitionViewModelBuilder gameDefinitionTransformation;
        internal ShowingXResultsMessageBuilder showingXResultsMessageBuilder;
        internal GameDefinitionCreator gameDefinitionCreator;

        public GameDefinitionController(DataContext dataContext,
            GameDefinitionRetriever gameDefinitionRetriever,
            GameDefinitionViewModelBuilder gameDefinitionTransformation,
            ShowingXResultsMessageBuilder showingXResultsMessageBuilder,
            GameDefinitionCreator gameDefinitionCreator)
        {
            this.dataContext = dataContext;
            this.gameDefinitionRetriever = gameDefinitionRetriever;
            this.gameDefinitionTransformation = gameDefinitionTransformation;
            this.showingXResultsMessageBuilder = showingXResultsMessageBuilder;
            this.gameDefinitionCreator = gameDefinitionCreator;
        }

        // GET: /GameDefinition/
        [Authorize]
        [UserContextAttribute]
        public virtual ActionResult Index(ApplicationUser currentUser)
        {
            IList<GameDefinition> games = gameDefinitionRetriever.GetAllGameDefinitions(currentUser.CurrentGamingGroupId.Value);
            return View(MVC.GameDefinition.Views.Index, games);
        }

        // GET: /GameDefinition/Details/5
        [UserContextAttribute(RequiresGamingGroup = false)]
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
                gameDefinition = gameDefinitionRetriever.GetGameDefinitionDetails(id.Value, NUMBER_OF_RECENT_GAMES_TO_SHOW);
                gameDefinitionViewModel = gameDefinitionTransformation.Build(gameDefinition, currentUser);
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
        [Authorize]
        public virtual ActionResult Create()
        {
            return View(MVC.GameDefinition.Views.Create);
        }

        // POST: /GameDefinition/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserContextAttribute]
        public virtual ActionResult Create([Bind(Include = "Id,Name,Description")] GameDefinition gameDefinition, ApplicationUser currentUser)
        {
            if (ModelState.IsValid)
            {
                gameDefinitionCreator.CreateGameDefinition(gameDefinition.Name, gameDefinition.Description, currentUser);
      
                return RedirectToAction(MVC.GameDefinition.ActionNames.Index);
            }

            return View(MVC.GameDefinition.Views.Create, gameDefinition);
        }

        // GET: /GameDefinition/Edit/5
        [Authorize]
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
                gameDefinition = gameDefinitionRetriever.GetGameDefinitionDetails(id.Value, 0);
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
        [Authorize]
        public virtual ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                GameDefinition gameDefinition = gameDefinitionRetriever.GetGameDefinitionDetails(
                    id.Value, 
                    0);
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
        [Authorize]
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
