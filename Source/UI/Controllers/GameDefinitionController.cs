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
		                new NewGameDefinitionViewModel()
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
		public virtual ActionResult Create(NewGameDefinitionViewModel newGameDefinitionViewModel, ApplicationUser currentUser)
		{
			if (ModelState.IsValid)
			{
                newGameDefinitionViewModel.Name = newGameDefinitionViewModel.Name.Trim();
                var gameDefinition = Mapper.Map<NewGameDefinitionViewModel, GameDefinition>(newGameDefinitionViewModel);

				var savedResult = gameDefinitionSaver.Save(gameDefinition, currentUser);

                if (!String.IsNullOrWhiteSpace(newGameDefinitionViewModel.ReturnUrl))
                    return new RedirectResult(newGameDefinitionViewModel.ReturnUrl + "?gameId=" + savedResult.Id);

				return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
										+ "#" + GamingGroupController.SECTION_ANCHOR_GAMEDEFINITIONS);
			}

            return View(MVC.GameDefinition.Views.Create, newGameDefinitionViewModel);
		}

		[Authorize]
		[HttpPost]
		[UserContext]
        //TODO vulnerable to overposting. Need to change this to a request type that only has the fields that can be edited here. This could also
        //probably be consolidated into one method, or just use the REST AIP
		public virtual ActionResult Save(GameDefinition model, ApplicationUser currentUser)
		{
			if (!Request.IsAjaxRequest())
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			if (ModelState.IsValid)
			{
				model.Name = model.Name.Trim();
				var game = gameDefinitionSaver.Save(model, currentUser);
				return Json(game, JsonRequestBehavior.AllowGet);
			}

			return new HttpStatusCodeResult(HttpStatusCode.NotModified);
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

			GameDefinition gameDefinition;
			try
			{
				gameDefinition = gameDefinitionRetriever.GetGameDefinitionDetails(id.Value, 0);
			}
			catch (KeyNotFoundException)
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
		[UserContext]
		public virtual ActionResult Edit([Bind(Include = "Id,Name,BoardGameGeekGameDefinitionId,Description,GamingGroupId,Active")] GameDefinition gamedefinition, ApplicationUser currentUser)
		{
			if (ModelState.IsValid)
			{
				gamedefinition.Name = gamedefinition.Name.Trim();
                gameDefinitionSaver.Save(gamedefinition, currentUser);
				dataContext.CommitAllChanges();
				return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
										  + "#" + GamingGroupController.SECTION_ANCHOR_GAMEDEFINITIONS);
			}
			return View(MVC.GameDefinition.Views.Edit, gamedefinition);
		}

		[Authorize]
		public virtual ActionResult CreatePartial()
		{
			return View(MVC.GameDefinition.Views._CreatePartial, new NewGameDefinitionViewModel());
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
