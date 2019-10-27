﻿﻿#region LICENSE
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

using System;
using System.Globalization;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.User;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BusinessLogic.Models.Players;
using BusinessLogic.Paging;
using UI.Attributes.Filters;
using UI.Controllers.Helpers;
using UI.Mappers.Interfaces;
using UI.Models;
using UI.Models.GameDefinitionModels;
using UI.Models.PlayedGame;
using UI.Models.Points;
using UI.Transformations;

namespace UI.Controllers
{
    public partial class PlayedGameController : BaseController
    {
        private readonly NemeStatsDataContext _dataContext;
        private readonly IPlayedGameRetriever _playedGameRetriever;
        private readonly IPlayerRetriever _playerRetriever;
        private readonly IPlayedGameDetailsViewModelBuilder _playedGameDetailsBuilder;
        private readonly IPlayedGameSaver _playedGameSaver;
        private readonly IGameDefinitionRetriever _gameDefinitionRetriever;
        private readonly IPlayedGameDeleter _playedGameDeleter;
        private readonly IPlayerSaver _playerSaver;
        private readonly IMapperFactory _mapperFactory;
        private readonly ICreatePlayedGameComponent _createPlayedGameComponent;
        private readonly ICreateGameDefinitionComponent _createGameDefinitionComponent;

        internal const int NUMBER_OF_RECENT_GAMES_TO_DISPLAY = 25;

        public PlayedGameController(
            NemeStatsDataContext dataContext,
            IPlayedGameRetriever playedGameRetriever,
            IPlayerRetriever playerRetriever,
            IPlayedGameDetailsViewModelBuilder builder,
            IGameDefinitionRetriever gameDefinitionRetriever,
            IPlayedGameSaver playedGameSaver,
            IPlayedGameDeleter playedGameDeleter,
            IPlayerSaver playerSaver,
            IMapperFactory mapperFactory, 
            ICreatePlayedGameComponent createPlayedGameComponent, 
            ICreateGameDefinitionComponent createGameDefinitionComponent)
        {
            _dataContext = dataContext;
            _playedGameRetriever = playedGameRetriever;
            _playerRetriever = playerRetriever;
            _playedGameDetailsBuilder = builder;
            _gameDefinitionRetriever = gameDefinitionRetriever;
            _playedGameSaver = playedGameSaver;
            _playedGameDeleter = playedGameDeleter;
            _playerSaver = playerSaver;
            _mapperFactory = mapperFactory;
            _createPlayedGameComponent = createPlayedGameComponent;
            _createGameDefinitionComponent = createGameDefinitionComponent;
        }

        // GET: /PlayedGame/Details/5
        [UserContext(RequiresGamingGroup = false)]
        public virtual ActionResult Details(int? id, ApplicationUser currentUser)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var playedGame = _playedGameRetriever.GetPlayedGameDetails(id.Value);
            if (playedGame == null)
            {
                return HttpNotFound();
            }
            var playedGameDetails = _playedGameDetailsBuilder.Build(playedGame, currentUser, true);
            return View(MVC.PlayedGame.Views.Details, playedGameDetails);
        }

        // GET: /PlayedGame/Create
        [System.Web.Mvc.Authorize]
        [UserContext]
        [System.Web.Mvc.HttpGet]
        public virtual ActionResult Create(ApplicationUser currentUser)
        {
            var viewModel = MakeBaseCreatePlayedGameViewModel<CreatePlayedGameViewModel>(currentUser.CurrentGamingGroupId.Value);

            var players = _playerRetriever.GetPlayersToCreate(currentUser.Id, currentUser.CurrentGamingGroupId.Value);
            viewModel.UserPlayer = players.UserPlayer;
            viewModel.OtherPlayers = players.OtherPlayers;
            viewModel.RecentPlayers = players.RecentPlayers;

            return View(MVC.PlayedGame.Views.CreateOrEdit, viewModel);
        }

        [NonAction]
        internal virtual T MakeBaseCreatePlayedGameViewModel<T>(int currentGamingGroupId) where T : CreatePlayedGameViewModel, new()
        {
            var mostPlayedGames =
                _gameDefinitionRetriever.GetMostPlayedGames(new GetMostPlayedGamesQuery
                {
                    GamingGroupId = currentGamingGroupId,
                    Page = 1,
                    PageSize = 5
                });
            var recentPlayedGames =
                _gameDefinitionRetriever.GetRecentGames(new GetRecentPlayedGamesQuery
                {
                    GamingGroupId = currentGamingGroupId,
                    Page = 1,
                    PageSize = 5
                });

            var recentPlayedGamesViewModels = _mapperFactory.GetMapper<GameDefinitionDisplayInfo, GameDefinitionDisplayInfoViewModel>()
                .Map(recentPlayedGames)
                .ToList();
            var mostPlayedGamesViewModels = _mapperFactory.GetMapper<GameDefinitionDisplayInfo, GameDefinitionDisplayInfoViewModel>()
                .Map(mostPlayedGames)
                .ToList();
            return new T
            {
                RecentPlayedGames = recentPlayedGamesViewModels,
                MostPlayedGames = mostPlayedGamesViewModels
            };
        }

        [NonAction]
        [Obsolete]
        internal virtual CreatePlayedGameViewModel FillCreatePlayedGameViewModel(ApplicationUser currentUser, CreatePlayedGameViewModel viewModel, bool forEdit = false)
        {
            var mostPlayedGames =
                _gameDefinitionRetriever.GetMostPlayedGames(new GetMostPlayedGamesQuery
                {
                    GamingGroupId = currentUser.CurrentGamingGroupId.Value,
                    Page = 1,
                    PageSize = 5
                });
            var recentPlayedGames =
                _gameDefinitionRetriever.GetRecentGames(new GetRecentPlayedGamesQuery
                {
                    GamingGroupId = currentUser.CurrentGamingGroupId.Value,
                    Page = 1,
                    PageSize = 5
                });

            var players = _playerRetriever.GetPlayersToCreate(currentUser.Id, currentUser.CurrentGamingGroupId.Value);

            viewModel.UserPlayer = players.UserPlayer;
            viewModel.OtherPlayers = players.OtherPlayers;
            viewModel.RecentPlayers = players.RecentPlayers;
            viewModel.RecentPlayedGames = _mapperFactory.GetMapper<GameDefinitionDisplayInfo, GameDefinitionDisplayInfoViewModel>()
                .Map(recentPlayedGames)
                .ToList();
            viewModel.MostPlayedGames = _mapperFactory.GetMapper<GameDefinitionDisplayInfo, GameDefinitionDisplayInfoViewModel>()
                .Map(mostPlayedGames)
                .ToList();
            return viewModel;
        }

        // POST: /PlayedGame/Save
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserContext]
        public virtual ActionResult Save(SavePlayedGameRequest request, ApplicationUser currentUser)
        {
            if (ModelState.IsValid)
            {
                int? resultId;

                if (request.GameDefinitionId == null)
                {
                    if (string.IsNullOrEmpty(request.GameDefinitionName))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }

                    request.GameDefinitionId = _createGameDefinitionComponent.Execute(new CreateGameDefinitionRequest
                    {
                        Name = request.GameDefinitionName,
                        GamingGroupId = request.GameDefinitionId ?? currentUser.CurrentGamingGroupId,
                        BoardGameGeekGameDefinitionId = request.BoardGameGeekGameDefinitionId
                    }, currentUser).Id;
                }

                if (request.EditMode && request.PlayedGameId.HasValue)
                {
                    _playedGameSaver.UpdatePlayedGame(
                        _mapperFactory.GetMapper<SavePlayedGameRequest, UpdatedGame>().Map(request),
                        TransactionSource.WebApplication, currentUser);

                    resultId = request.PlayedGameId;
                }
                else
                {
                    var newlyCompletedGame = _mapperFactory.GetMapper<SavePlayedGameRequest, NewlyCompletedGame>().Map(request);
                    newlyCompletedGame.TransactionSource = TransactionSource.WebApplication;
                    resultId =
                        _createPlayedGameComponent.Execute(newlyCompletedGame, currentUser).Id;
                }


                return Json(new { success = true, playedGameId = resultId });
            }

            return Json(new { success = false, errors = ModelState.Errors() }, JsonRequestBehavior.AllowGet);
        }

        // GET: /PlayedGame/Edit
        [Authorize]
        [UserContext]
        [HttpGet]
        public virtual ActionResult Edit(int id, ApplicationUser currentUser)
        {
            var viewModel = MakeBaseCreatePlayedGameViewModel<EditPlayedGameViewModel>(currentUser.CurrentGamingGroupId.Value);

            viewModel.EditMode = true;
            viewModel.PlayedGameId = id;

            var playedGameInfo = _playedGameRetriever.GetInfoForEditingPlayedGame(id, currentUser);

            viewModel.OtherPlayers = playedGameInfo.OtherPlayers;
            viewModel.RecentPlayers = playedGameInfo.RecentPlayers;
            viewModel.UserPlayer = playedGameInfo.UserPlayer;

            viewModel.DatePlayed = playedGameInfo.DatePlayed;
            viewModel.Notes = playedGameInfo.Notes;
            viewModel.GameDefinitionId = playedGameInfo.GameDefinitionId;
            viewModel.GameDefinitionName = playedGameInfo.GameDefinitionName;
            viewModel.BoardGameGeekGameDefinitionId = playedGameInfo.BoardGameGeekGameDefinitionId;
            viewModel.WinnerType = playedGameInfo.WinnerType;
            viewModel.GameType = SetGameType(playedGameInfo.PlayerRanks);

            viewModel.PlayerRanks = playedGameInfo.PlayerRanks;
            
            return View(MVC.PlayedGame.Views.CreateOrEdit, viewModel);
//TODO allow editing of a game immediately after saving. No need to disable if currentStep == 5
        }

        private GameResultTypes SetGameType(List<PlayerRankWithName> playerRanks)
        {
            if (playerRanks.All(x => x.PointsScored.HasValue) && playerRanks.Any(x => x.PointsScored != 0))
            {
                return GameResultTypes.Scored;
            }

            var firstRank = playerRanks.First().GameRank;
            if (playerRanks.All(x => x.GameRank == firstRank))
            {
                return GameResultTypes.Cooperative;
            }

            return GameResultTypes.Ranked;
        }


        [System.Web.Mvc.HttpGet]
        public virtual ActionResult ShowRecentlyPlayedGames()
        {
            var recentlyPlayedGamesFilter = new RecentlyPlayedGamesFilter
            {
                NumberOfGamesToRetrieve = NUMBER_OF_RECENT_GAMES_TO_DISPLAY,
                //--only include games that are tomorrow or in the past to avoid issues with the Thai Buddhist calendar
                MaxDate = DateTime.UtcNow.Date.AddDays(1),
                MinDate = DateTime.UtcNow.Date.AddDays(-7)
            };
            var recentlyPlayedGames = _playedGameRetriever.GetRecentPublicGames(recentlyPlayedGamesFilter);

            return View(MVC.PlayedGame.Views.RecentlyPlayedGames, recentlyPlayedGames);
        }

        // GET: /PlayedGame/Delete/5
        [System.Web.Mvc.Authorize]
        [UserContext]
        public virtual ActionResult Delete(int? id, ApplicationUser currentUser)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var playedgame = _dataContext.GetQueryable<PlayedGame>().FirstOrDefault(playedGame => playedGame.Id == id.Value);
            if (playedgame == null)
            {
                return HttpNotFound();
            }
            return View(playedgame);
        }

        // POST: /PlayedGame/Delete/5
        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpPost, System.Web.Mvc.ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [UserContext]
        public virtual ActionResult DeleteConfirmed(int id, ApplicationUser currentUser)
        {
            _playedGameDeleter.DeletePlayedGame(id, currentUser);

            return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
                            + "#" + GamingGroupController.SECTION_ANCHOR_RECENT_GAMES);
        }

        // POST: /PlayedGame/DangerousDelete/5
        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpPost, System.Web.Mvc.ActionName("DangerousDelete")]
        [ValidateAntiForgeryToken]
        [UserContext]
        public virtual ActionResult DangerousDelete(int id, ApplicationUser currentUser)
        {
            _playedGameDeleter.DeletePlayedGame(id, currentUser);
            return new RedirectResult(Request.UrlReferrer.ToString());
        }

        [System.Web.Mvc.Authorize]
        [UserContext]
        [System.Web.Mvc.HttpGet]
        public virtual ActionResult Search(ApplicationUser currentUser)
        {
            var viewModel = new SearchViewModel
            {
                GameDefinitions = GetAllGameDefinitionsForCurrentGamingGroup(currentUser.CurrentGamingGroupId.Value)
            };

            AddPlayersToViewModel(currentUser.CurrentGamingGroupId.Value, viewModel, null);


            return View(MVC.PlayedGame.Views.Search, viewModel);
        }

        [System.Web.Mvc.Authorize]
        [UserContext]
        [System.Web.Mvc.HttpPost]
        public virtual ActionResult Search(PlayedGamesFilterViewModel filter, ApplicationUser currentUser)
        {
            var playedGameFilter = new PlayedGameFilter
            {
                EndDateGameLastUpdated = filter.DatePlayedEnd?.ToString("yyyy-MM-dd"),
                GamingGroupId = currentUser.CurrentGamingGroupId,
                StartDateGameLastUpdated = filter.DatePlayedStart?.ToString("yyyy-MM-dd"),
                GameDefinitionId = filter.GameDefinitionId,
                PlayerId = filter.IncludedPlayerId
            };
            var searchResults = _playedGameRetriever.SearchPlayedGames(playedGameFilter);

            var playedGamesDetails = searchResults.Select(searchResult => new PlayedGameDetailsViewModel
            {
                DatePlayed = searchResult.DatePlayed,
                GameDefinitionId = searchResult.GameDefinitionId,
                GameDefinitionName = searchResult.GameDefinitionName,
                GamingGroupId = searchResult.GamingGroupId,
                GamingGroupName = searchResult.GamingGroupName,
                Notes = searchResult.Notes,
                PlayedGameId = searchResult.PlayedGameId,
                UserCanEdit = true,
                WinnerType = searchResult.WinnerType,
                PlayerResults = searchResult.PlayerGameResults.Select(playerResult => new GameResultViewModel
                {
                    DatePlayed = searchResult.DatePlayed,
                    GameDefinitionId = searchResult.GameDefinitionId,
                    GameDefinitionName = searchResult.GameDefinitionName,
                    GameRank = playerResult.GameRank,
                    NemePointsSummary = new NemePointsSummaryViewModel(playerResult.NemeStatsPointsAwarded, playerResult.GameDurationBonusNemePoints, playerResult.GameWeightBonusNemePoints),
                    PlayedGameId = searchResult.PlayedGameId,
                    PlayerId = playerResult.PlayerId,
                    PlayerName = playerResult.PlayerName,
                    PointsScored = playerResult.PointsScored,
                    WinnerType = searchResult.WinnerType
                }).ToList()
            }).ToList();

            var viewModel = new SearchViewModel
            {
                Filter =
                {
                    DatePlayedEnd = filter.DatePlayedEnd,
                    DatePlayedStart = filter.DatePlayedStart,
                    GameDefinitionId = filter.GameDefinitionId
                },
                GameDefinitions = GetAllGameDefinitionsForCurrentGamingGroup(currentUser.CurrentGamingGroupId.Value),
                PlayedGames = new PlayedGamesViewModel
                {
                    PlayedGameDetailsViewModels = playedGamesDetails,
                    UserCanEdit = true,
                    GamingGroupId = currentUser.CurrentGamingGroupId,
                    ShowSearchLinkInResultsHeader = false
                }
            };

            AddPlayersToViewModel(currentUser.CurrentGamingGroupId.Value, viewModel, filter.IncludedPlayerId);

            return View(MVC.PlayedGame.Views.Search, viewModel);
        }

        private IList<SelectListItem> GetAllGameDefinitionsForCurrentGamingGroup(int gamingGroupId)
        {
            var gameDefinitions = _gameDefinitionRetriever.GetAllGameDefinitionNames(gamingGroupId);
            var selectListItems = new List<SelectListItem>
            {
                new SelectListItem
                {
                   Selected = true,
                    Text = "All",
                    Value = string.Empty
                }

            };

            selectListItems.AddRange(gameDefinitions
                .OrderBy(gameDefinition => gameDefinition.Name)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(CultureInfo.InvariantCulture)
                }).ToList());

            return selectListItems;
        }

        internal virtual void AddPlayersToViewModel(int currentGamingGroupId, SearchViewModel searchViewModel, int? selectedPlayerId)
        {
            var players = _playerRetriever.GetAllPlayers(currentGamingGroupId, false);
            var playerSelectListItems = players.Select(player => new SelectListItem
            {
                Text = player.Name,
                Value = player.Id.ToString(),
                Selected = player.Id == selectedPlayerId
            }).ToList();

            searchViewModel.Players = playerSelectListItems;
        }
    }
}
