#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using AutoMapper;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using BoardGameGeekApiClient.Interfaces;
using UI.Attributes.Filters;
using UI.Controllers.Helpers;
using UI.Models.GameDefinitionModels;
using UI.Transformations;
using BusinessLogic.Exceptions;

namespace UI.Controllers
{
    public partial class GameDefinitionController : Controller
    {
        internal const int NUMBER_OF_RECENT_GAMES_TO_SHOW = 5;

        internal IDataContext dataContext;
        internal IGameDefinitionRetriever gameDefinitionRetriever;
        internal IGameDefinitionDetailsViewModelBuilder gameDefinitionTransformation;
        internal IShowingXResultsMessageBuilder showingXResultsMessageBuilder;
        internal IGameDefinitionSaver gameDefinitionSaver;
        internal IBoardGameGeekApiClient _boardGameGeekApiClient;

        public GameDefinitionController(IDataContext dataContext,
            IGameDefinitionRetriever gameDefinitionRetriever,
            IGameDefinitionDetailsViewModelBuilder gameDefinitionTransformation,
            IShowingXResultsMessageBuilder showingXResultsMessageBuilder,
            IGameDefinitionSaver gameDefinitionCreator,
            IBoardGameGeekApiClient boardGameGeekApiClient)
        {
            this.dataContext = dataContext;
            this.gameDefinitionRetriever = gameDefinitionRetriever;
            this.gameDefinitionTransformation = gameDefinitionTransformation;
            this.showingXResultsMessageBuilder = showingXResultsMessageBuilder;
            this.gameDefinitionSaver = gameDefinitionCreator;
            _boardGameGeekApiClient = boardGameGeekApiClient;
        }

        // GET: /GameDefinition/Details/5
        [UserContext(RequiresGamingGroup = false)]
        public virtual ActionResult Details(int? id, ApplicationUser currentUser)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GameDefinitionSummary gameDefinitionSummary;
            GameDefinitionDetailsViewModel gamingGroupGameDefinitionViewModel;

            try
            {
                gameDefinitionSummary = gameDefinitionRetriever.GetGameDefinitionDetails(id.Value, NUMBER_OF_RECENT_GAMES_TO_SHOW);
                gamingGroupGameDefinitionViewModel = gameDefinitionTransformation.Build(gameDefinitionSummary, currentUser);
            }
            catch (KeyNotFoundException)
            {
                return new HttpNotFoundResult();
            }
            catch (UnauthorizedAccessException)
            {
                return new HttpUnauthorizedResult();
            }

            gamingGroupGameDefinitionViewModel.PlayedGamesPanelTitle =
                string.Format("Last {0} Played Games", gamingGroupGameDefinitionViewModel.PlayedGames.Count);

            return View(MVC.GameDefinition.Views.Details, gamingGroupGameDefinitionViewModel);
        }

        // GET: /GameDefinition/Create
        [Authorize]
        public virtual ActionResult Create(string returnUrl)
        {
            return View(MVC.GameDefinition.Views.Create,
                        new CreateGameDefinitionViewModel()
                        {
                            ReturnUrl = returnUrl
                        });
        }

        // POST: /GameDefinition/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserContext]
        public virtual ActionResult Create(CreateGameDefinitionViewModel createGameDefinitionViewModel, ApplicationUser currentUser)
        {
            if (ModelState.IsValid)
            {
                createGameDefinitionViewModel.Name = createGameDefinitionViewModel.Name.Trim();
                var gameDefinition = Mapper.Map<CreateGameDefinitionViewModel, CreateGameDefinitionRequest>(createGameDefinitionViewModel);

                var savedResult = gameDefinitionSaver.CreateGameDefinition(gameDefinition, currentUser);

                if (!String.IsNullOrWhiteSpace(createGameDefinitionViewModel.ReturnUrl))
                    return new RedirectResult(createGameDefinitionViewModel.ReturnUrl + "?gameId=" + savedResult.Id);

                return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
                                        + "#" + GamingGroupController.SECTION_ANCHOR_GAMEDEFINITIONS);
            }

            return View(MVC.GameDefinition.Views.Create, createGameDefinitionViewModel);
        }

        [Authorize]
        [HttpPost]
        [UserContext]
        public virtual ActionResult AjaxCreate(CreateGameDefinitionViewModel createGameDefinitionViewModel, ApplicationUser currentUser)
        {
            if (!Request.IsAjaxRequest())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                createGameDefinitionViewModel.Name = createGameDefinitionViewModel.Name.Trim();
                var createGameDefinitionRequest = Mapper.Map<CreateGameDefinitionRequest>(createGameDefinitionViewModel);

                GameDefinition newGameDefinition;
                try
                {
                    newGameDefinition = gameDefinitionSaver.CreateGameDefinition(createGameDefinitionRequest, currentUser);
                }
                catch (DuplicateKeyException)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "This Game Definition is already active within your Gaming Group.");
                }
                return Json(newGameDefinition, JsonRequestBehavior.AllowGet);
            }

            string errorDescription = string.Empty;

            errorDescription = GetFirstModelStateError(errorDescription);
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, errorDescription);
        }

        private string GetFirstModelStateError(string errorDescription)
        {
            foreach (var modelStateValue in ModelState.Values)
            {
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        errorDescription = error.ErrorMessage;
                    }
                }
            }

            return errorDescription;
        }

        private static GameDefinitionUpdateRequest TransformGameDefinitionEditViewModelToGameDefinitionUpdateRequest(GameDefinitionEditViewModel model)
        {
            return new GameDefinitionUpdateRequest
            {
                GameDefinitionId = model.GameDefinitionId,
                Active = model.Active,
                BoardGameGeekGameDefinitionId = model.BoardGameGeekGameDefinitionId,
                Name = model.Name,
                Description = model.Description
            };
        }

        // GET: /GameDefinition/Edit/5
        [Authorize]
        [UserContext]
        public virtual ActionResult Edit(int? id, ApplicationUser currentUser)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GameDefinitionEditViewModel gameDefinitionEditViewModel;
            try
            {
                var gameDefinition = gameDefinitionRetriever.GetGameDefinitionDetails(id.Value, 0);
                gameDefinitionEditViewModel = Mapper.Map<GameDefinitionEditViewModel>(gameDefinition);
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }

            return View(MVC.GameDefinition.Views.Edit, gameDefinitionEditViewModel);
        }

        // POST: /GameDefinition/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        [UserContext]
        public virtual ActionResult Edit(GameDefinitionEditViewModel viewModel, ApplicationUser currentUser)
        {
            if (ModelState.IsValid)
            {
                viewModel.Name = viewModel.Name.Trim();
                var gameDefinitionUpdateRequest = Mapper.Map<GameDefinitionUpdateRequest>(viewModel);
                gameDefinitionSaver.UpdateGameDefinition(gameDefinitionUpdateRequest, currentUser);
                return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
                                          + "#" + GamingGroupController.SECTION_ANCHOR_GAMEDEFINITIONS);
            }
            return View(MVC.GameDefinition.Views.Edit, viewModel);
        }

        [Authorize]
        public virtual ActionResult CreatePartial()
        {
            return View(MVC.GameDefinition.Views._CreatePartial, new CreateGameDefinitionViewModel());
        }

        [Authorize]
        [HttpGet]
        [UserContext]
        public virtual ActionResult SearchBoardGameGeekHttpGet(string searchText)
        {
            if (!Request.IsAjaxRequest())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                var searchResults = _boardGameGeekApiClient.SearchBoardGames(searchText);
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
