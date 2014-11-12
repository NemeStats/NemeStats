using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Models;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using UI.Filters;
using BusinessLogic.Logic.GameDefinitions;
using UI.Transformations;
using UI.Models.GameDefinitionModels;
using UI.Controllers.Helpers;

namespace UI.Controllers
{
    public partial class GameDefinitionController : Controller
    {
        internal const int NUMBER_OF_RECENT_GAMES_TO_SHOW = 5;

        internal IDataContext dataContext;
        internal IGameDefinitionRetriever gameDefinitionRetriever;
        internal IGameDefinitionViewModelBuilder gameDefinitionTransformation;
        internal IShowingXResultsMessageBuilder showingXResultsMessageBuilder;
        internal IGameDefinitionSaver gameDefinitionSaver;
        internal IBoardGameGeekSearcher boardGameGeekSearcher;
        public const int MIN_LENGTH_FOR_PARTIAL_MATCH_BOARD_GAME_GEEK_SEARCH = 5;

        public GameDefinitionController(IDataContext dataContext,
            IGameDefinitionRetriever gameDefinitionRetriever,
            IGameDefinitionViewModelBuilder gameDefinitionTransformation,
            IShowingXResultsMessageBuilder showingXResultsMessageBuilder,
            IGameDefinitionSaver gameDefinitionCreator,
            IBoardGameGeekSearcher boardGameGeekSearcher)
        {
            this.dataContext = dataContext;
            this.gameDefinitionRetriever = gameDefinitionRetriever;
            this.gameDefinitionTransformation = gameDefinitionTransformation;
            this.showingXResultsMessageBuilder = showingXResultsMessageBuilder;
            this.gameDefinitionSaver = gameDefinitionCreator;
            this.boardGameGeekSearcher = boardGameGeekSearcher;
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
        public virtual ActionResult Create([Bind(Include = "Id,Name,Description,Active")] GameDefinition gameDefinition, ApplicationUser currentUser)
        {
            if (ModelState.IsValid)
            {
                gameDefinitionSaver.Save(gameDefinition, currentUser);
      
                return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name) 
                                            + "#" + GamingGroupController.SECTION_ANCHOR_GAMEDEFINITIONS);
            }

            return View(MVC.GameDefinition.Views.Create, gameDefinition);
        }

        [Authorize]
        [HttpPost]
        [UserContextAttribute]
        public virtual ActionResult Save(GameDefinition model, ApplicationUser currentUser)
        {
            if (!Request.IsAjaxRequest())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                    GameDefinition game = gameDefinitionSaver.Save(model, currentUser);
                    return Json(game, JsonRequestBehavior.AllowGet);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotModified);
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
        public virtual ActionResult Edit([Bind(Include = "Id,Name,Description,GamingGroupId,Active")] GameDefinition gamedefinition, ApplicationUser currentUser)
        {
            if (ModelState.IsValid)
            {
                dataContext.Save(gamedefinition, currentUser);
                dataContext.CommitAllChanges();
                return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
                                          + "#" + GamingGroupController.SECTION_ANCHOR_GAMEDEFINITIONS);
            }
            return View(MVC.GameDefinition.Views.Edit, gamedefinition);
        }

        [Authorize]
        public virtual ActionResult CreatePartial()
        {
            return View(MVC.GameDefinition.Views._CreatePartial, new GameDefinition());
        }

        [Authorize]
        [HttpGet]
        [UserContextAttribute]
        public virtual ActionResult SearchBoardGameGeekHttpGet(string searchText)
        {
            if (!Request.IsAjaxRequest())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                //Jake, I disabled this because it is not needed anymore.
                bool requireExactMatches = false;//searchText.Length < MIN_LENGTH_FOR_PARTIAL_MATCH_BOARD_GAME_GEEK_SEARCH;
                List<BoardGameGeekSearchResult> searchResults = boardGameGeekSearcher.SearchForBoardGames(searchText, requireExactMatches);
                return Json(searchResults, JsonRequestBehavior.AllowGet);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotModified);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
