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

#endregion LICENSE

using AutoMapper;
using BoardGameGeekApiClient.Interfaces;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BusinessLogic.Facades;
using BusinessLogic.Logic;
using UI.Attributes.Filters;
using UI.Controllers.Helpers;
using UI.Models.GameDefinitionModels;
using UI.Transformations;

namespace UI.Controllers
{
    public partial class GameDefinitionController : BaseController
    {
        internal const int NUMBER_OF_RECENT_GAMES_TO_SHOW = 5;
        internal const int NUMBER_OF_TRENDING_GAMES_TO_SHOW = 25;
        internal const int NUMBER_OF_TOP_GAMES_EVER_TO_SHOW = 25;
        internal const int NUMBER_OF_DAYS_OF_TRENDING_GAMES = 90;
        internal const int A_LOT_OF_DAYS = 10000;

        private readonly IGameDefinitionRetriever _gameDefinitionRetriever;
        private readonly ITrendingGamesRetriever _trendingGamesRetriever;
        private readonly IGameDefinitionDetailsViewModelBuilder _gameDefinitionTransformation;
        private readonly IGameDefinitionSaver _gameDefinitionSaver;
        private readonly IBoardGameGeekApiClient _boardGameGeekApiClient;
        private readonly IUserRetriever _userRetriever;
        private readonly IBoardGameGeekGamesImporter _boardGameGeekGamesImporter;
        private readonly ITransformer _transformer;
        private readonly ICreateGameDefinitionComponent _createGameDefinitionComponent;

        public GameDefinitionController(IGameDefinitionRetriever gameDefinitionRetriever,
            ITrendingGamesRetriever trendingGamesRetriever,
            IGameDefinitionDetailsViewModelBuilder gameDefinitionTransformation,
            IGameDefinitionSaver gameDefinitionCreator,
            IBoardGameGeekApiClient boardGameGeekApiClient,
            IUserRetriever userRetriever,
            IBoardGameGeekGamesImporter boardGameGeekGamesImporter,
            ITransformer transformer, ICreateGameDefinitionComponent createGameDefinitionComponent)
        {
            _gameDefinitionRetriever = gameDefinitionRetriever;
            _trendingGamesRetriever = trendingGamesRetriever;
            _gameDefinitionTransformation = gameDefinitionTransformation;
            _gameDefinitionSaver = gameDefinitionCreator;
            _boardGameGeekApiClient = boardGameGeekApiClient;
            _userRetriever = userRetriever;
            _boardGameGeekGamesImporter = boardGameGeekGamesImporter;
            _transformer = transformer;
            _createGameDefinitionComponent = createGameDefinitionComponent;
        }

        // GET: /GameDefinition/Details/5
        [UserContext(RequiresGamingGroup = false)]
        public virtual ActionResult Details(int? id, ApplicationUser currentUser)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GameDefinitionDetailsViewModel gamingGroupGameDefinitionViewModel;

            try
            {
                var gameDefinitionSummary = _gameDefinitionRetriever.GetGameDefinitionDetails(id.Value, NUMBER_OF_RECENT_GAMES_TO_SHOW);
                gamingGroupGameDefinitionViewModel = _gameDefinitionTransformation.Build(gameDefinitionSummary, currentUser);
            }
            catch (KeyNotFoundException)
            {
                return new HttpNotFoundResult();
            }
            catch (UnauthorizedAccessException)
            {
                return new HttpUnauthorizedResult();
            }

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
                var createGameDefinitionRequest = Mapper.Map<CreateGameDefinitionViewModel, CreateGameDefinitionRequest>(createGameDefinitionViewModel);

                var savedResult = _createGameDefinitionComponent.Execute(createGameDefinitionRequest, currentUser);

                if (!string.IsNullOrWhiteSpace(createGameDefinitionViewModel.ReturnUrl))
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

                try
                {
                    _createGameDefinitionComponent.Execute(createGameDefinitionRequest, currentUser);
                }
                catch (DuplicateKeyException)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "This Game Definition is already active within your Gaming Group.");
                }
                return Json(currentUser.CurrentGamingGroupId, JsonRequestBehavior.AllowGet);
            }

            var errorDescription = string.Empty;

            errorDescription = GetFirstModelStateError(errorDescription);
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, errorDescription);
        }

        [Authorize]
        [HttpPost]
        [UserContext]
        public virtual ActionResult ImportFromBGG(ApplicationUser currentUser)
        {
            var gamesImported = _boardGameGeekGamesImporter.ImportBoardGameGeekGames(currentUser);

            if (gamesImported == null)
            {
                SetToastMessage(TempMessageKeys.CREATE_GAMEDEFITION_RESULT_TEMPMESSAGE, "It appears as though you don't have any games in your BoardGameGeek collection :_(", "info");
            }
            else if (gamesImported == 0)
            {
                SetToastMessage(TempMessageKeys.CREATE_GAMEDEFITION_RESULT_TEMPMESSAGE,
                    "All your BoardGameGeek games are already imported ;-)", "info");
            }
            else
            {
                SetToastMessage(TempMessageKeys.CREATE_GAMEDEFITION_RESULT_TEMPMESSAGE, $"{gamesImported} games imported from your BoardGameGeek collection to NemeStats. Awesome!");
            }

            return RedirectToAction(MVC.GamingGroup.Index());
        }

        private string GetFirstModelStateError(string errorDescription)
        {
            foreach (var modelStateValue in ModelState.Values)
            {
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        errorDescription = error.ErrorMessage;
                    }
                }
            }

            return errorDescription;
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
                var gameDefinition = _gameDefinitionRetriever.GetGameDefinitionDetails(id.Value, 0);
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
                _gameDefinitionSaver.UpdateGameDefinition(gameDefinitionUpdateRequest, currentUser);
                return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
                                          + "#" + GamingGroupController.SECTION_ANCHOR_GAMEDEFINITIONS);
            }
            return View(MVC.GameDefinition.Views.Edit, viewModel);
        }

        [Authorize]
        [UserContext]
        public virtual ActionResult CreatePartial(ApplicationUser currentUser)
        {
            var bggUser = _userRetriever.RetrieveUserInformation(currentUser).BoardGameGeekUser;

            return View(MVC.GameDefinition.Views._CreatePartial, new CreateGameDefinitionViewModel() { BGGUserName = bggUser?.Name, GamingGroupId = currentUser.CurrentGamingGroupId.Value });
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

        [Authorize]
        [HttpGet]
        [UserContext]
        public virtual ActionResult SearchGameDefinition(string q, ApplicationUser currentUser)
        {
            if (!Request.IsAjaxRequest())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IList<GameDefinitionName> searchResults;
            if (currentUser.CurrentGamingGroupId.HasValue)
            {
                searchResults = _gameDefinitionRetriever.GetAllGameDefinitionNames(currentUser.CurrentGamingGroupId.Value, q);
            }
            else
            {
                searchResults = new List<GameDefinitionName>();
            }

            return Json(searchResults, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public virtual ActionResult ShowTrendingGames()
        {
            var trendingGamesViewModels = GetTrendingGamesViewModels(NUMBER_OF_TRENDING_GAMES_TO_SHOW, NUMBER_OF_DAYS_OF_TRENDING_GAMES);
            ViewBag.NumTrendingDays = NUMBER_OF_DAYS_OF_TRENDING_GAMES;

            return View(MVC.GameDefinition.Views.TrendingGames, trendingGamesViewModels);
        }

        [NonAction]
        internal virtual List<TrendingGameViewModel> GetTrendingGamesViewModels(int numberOfGames, int numberOfDays)
        {
            var trendingGamesRequest =
                new TrendingGamesRequest(numberOfGames, numberOfDays);
            var trendingGames = _trendingGamesRetriever.GetResults(trendingGamesRequest);
            return trendingGames.Select(_transformer.Transform<TrendingGameViewModel>).ToList();
        }

        [HttpGet]
        public virtual ActionResult TopGames()
        {
            var trendingGamesViewModels = GetTrendingGamesViewModels(NUMBER_OF_TOP_GAMES_EVER_TO_SHOW, A_LOT_OF_DAYS);

            return View(MVC.GameDefinition.Views.TopGamesEver, trendingGamesViewModels);
        }
    }
}