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
using System.Web.Http;
using System.Web.Mvc;
using BusinessLogic.Paging;
using PagedList;
using UI.Attributes.Filters;
using UI.Controllers.Helpers;
using UI.Mappers;
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
        private readonly IPlayedGameCreator _playedGameCreator;
        private readonly IGameDefinitionRetriever _gameDefinitionRetriever;
        private readonly IPlayedGameDeleter _playedGameDeleter;
        private readonly GameDefinitionDisplayInfoToGameDefinitionDisplayInfoViewModelMapper _gameDefinitionDisplayInfoToGameDefinitionDisplayInfoViewModelMapper;

        internal const int NUMBER_OF_RECENT_GAMES_TO_DISPLAY = 25;

        public PlayedGameController(
            NemeStatsDataContext dataContext,
            IPlayedGameRetriever playedGameRetriever,
            IPlayerRetriever playerRetriever,
            IPlayedGameDetailsViewModelBuilder builder,
            IGameDefinitionRetriever gameDefinitionRetriever,
            IPlayedGameCreator playedGameCreator,
            IPlayedGameDeleter playedGameDeleter,
            GameDefinitionDisplayInfoToGameDefinitionDisplayInfoViewModelMapper gameDefinitionDisplayInfoToGameDefinitionDisplayInfoViewModelMapper)
        {
            _dataContext = dataContext;
            _playedGameRetriever = playedGameRetriever;
            _playerRetriever = playerRetriever;
            _playedGameDetailsBuilder = builder;
            _gameDefinitionRetriever = gameDefinitionRetriever;
            _playedGameCreator = playedGameCreator;
            _playedGameDeleter = playedGameDeleter;
            _gameDefinitionDisplayInfoToGameDefinitionDisplayInfoViewModelMapper = gameDefinitionDisplayInfoToGameDefinitionDisplayInfoViewModelMapper;
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
            var playedGameDetails = _playedGameDetailsBuilder.Build(playedGame, currentUser);
            return View(MVC.PlayedGame.Views.Details, playedGameDetails);
        }

        // GET: /PlayedGame/Create
        [System.Web.Mvc.Authorize]
        [UserContext]
        [System.Web.Mvc.HttpGet]
        public virtual ActionResult Create(ApplicationUser currentUser)
        {
            //var gameDefinitionSummaries = _gameDefinitionRetriever.GetAllGameDefinitions(currentUser.CurrentGamingGroupId);

            //var gameDefinitionSummariesSelectList = BuildGameDefinitionSummariesSelectList(gameDefinitionSummaries);
            //var viewModel = new PlayedGameEditViewModel
            //{
            //    GameDefinitions = gameDefinitionSummariesSelectList,
            //    Players = GetAllPlayers(currentUser)
            //};

            //return View(MVC.PlayedGame.Views.Create, viewModel);

            var mostPlayedGames = _gameDefinitionRetriever.GetMostPlayedGames(new GetMostPlayedGamesQuery { GamingGroupId = currentUser.CurrentGamingGroupId, Page = 1, PageSize = 5 });
            var recentPlayedGames = _gameDefinitionRetriever.GetRecentGames(new GetRecentPlayedGamesQuery { GamingGroupId = currentUser.CurrentGamingGroupId, Page = 1, PageSize = 5 });

            var players = _playerRetriever.GetPlayersToCreate(currentUser.Id);
            
            var viewModel = new CreatePlayedGameViewModel
            {
                MostPlayedGames = _gameDefinitionDisplayInfoToGameDefinitionDisplayInfoViewModelMapper.Map(mostPlayedGames).ToList(),
                RecentPlayedGames = _gameDefinitionDisplayInfoToGameDefinitionDisplayInfoViewModelMapper.Map(recentPlayedGames).ToList(),
                RecentPlayers = players.RecentPlayers,
                OtherPlayers = players.OtherPlayers,
                UserPlayer = players.UserPlayer
            };
            return View(MVC.PlayedGame.Views.Create, viewModel);
        }

        // POST: /PlayedGame/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [UserContext]
        public virtual ActionResult Create(NewlyCompletedGame newlyCompletedGame, [FromUri]bool recordAnotherGameAfterThis, ApplicationUser currentUser)
        {
            if (ModelState.IsValid)
            {
                _playedGameCreator.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, currentUser);

                if (!recordAnotherGameAfterThis)
                {
                    return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name) + "#" + GamingGroupController.SECTION_ANCHOR_RECENT_GAMES);
                }

                SetToastMessage(TempMessageKeys.TEMP_MESSAGE_KEY_PLAYED_GAME_RECORDED, "Played Game successfully recorded");
            }

            return Create(currentUser);
        }

        [System.Web.Mvc.HttpGet]
        public virtual ActionResult ShowRecentlyPlayedGames()
        {
            var recentlyPlayedGames = _playedGameRetriever.GetRecentPublicGames(NUMBER_OF_RECENT_GAMES_TO_DISPLAY);

            return View(MVC.PlayedGame.Views.RecentlyPlayedGames, recentlyPlayedGames);
        }

        private IEnumerable<SelectListItem> GetAllPlayers(ApplicationUser currentUser)
        {
            var allPlayers = _playerRetriever.GetAllPlayers(currentUser.CurrentGamingGroupId, false);
            var allPlayersSelectList = allPlayers.Select(item => new SelectListItem
            {
                Text = PlayerNameBuilder.BuildPlayerName(item.Name, item.Active),
                Value = item.Id.ToString()
            }).ToList();

            return allPlayersSelectList;
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
            //TODO really don't know whether I need to commit here or if it is automatically taken care of when disposing.
            _dataContext.CommitAllChanges();
            return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
                            + "#" + GamingGroupController.SECTION_ANCHOR_RECENT_GAMES);
        }

        // GET: /PlayedGame/Edit
        [System.Web.Mvc.Authorize]
        [UserContext]
        [System.Web.Mvc.HttpGet]
        public virtual ActionResult Edit(int id, ApplicationUser currentUser)
        {
            var viewModel = new PlayedGameEditViewModel();
            var gameDefinitionsList = _gameDefinitionRetriever.GetAllGameDefinitions(currentUser.CurrentGamingGroupId);
            viewModel.GameDefinitions = gameDefinitionsList.Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Id.ToString()
            }).ToList();
            viewModel.Players = GetAllPlayers(currentUser);

            if (id > 0)
            {
                var playedGame = _playedGameRetriever.GetPlayedGameDetails(id);
                viewModel.PreviousGameId = playedGame.Id;
                viewModel.GameDefinitionId = playedGame.GameDefinitionId;
                viewModel.DatePlayed = playedGame.DatePlayed;
                viewModel.Notes = playedGame.Notes;

                viewModel.PlayerRanks = playedGame.PlayerGameResults.Select(item => new PlayerRank { GameRank = item.GameRank, PlayerId = item.PlayerId }).ToList();
                viewModel.ExistingRankedPlayerNames = playedGame.PlayerGameResults.Select(item => new { item.Player.Name, item.Player.Id }).ToDictionary(p => p.Name, q => q.Id);
                viewModel.Players = RemovePlayersFromExistingPlayerRanks(viewModel.Players.ToList(), viewModel.PlayerRanks);
            }

            return View(viewModel);
        }

        // POST: /PlayedGame/Edit
        [System.Web.Mvc.Authorize]
        [UserContext]
        [System.Web.Mvc.HttpPost]
        public virtual ActionResult Edit(NewlyCompletedGame newlyCompletedGame, int previousGameId, ApplicationUser currentUser)
        {
            if (ModelState.IsValid)
            {
                _playedGameDeleter.DeletePlayedGame(previousGameId, currentUser);
                _playedGameCreator.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, currentUser);

                return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
                                            + "#" + GamingGroupController.SECTION_ANCHOR_RECENT_GAMES);
            }

            return RedirectToAction(MVC.PlayedGame.Edit(previousGameId, currentUser));
        }

        private IEnumerable<SelectListItem> RemovePlayersFromExistingPlayerRanks(IEnumerable<SelectListItem> players, List<PlayerRank> playerRanks)
        {
            return players.Where(item => playerRanks.Any(p => p.PlayerId.ToString() == item.Value) == false).ToList();
        }

        [System.Web.Mvc.Authorize]
        [UserContext]
        [System.Web.Mvc.HttpGet]
        public virtual ActionResult Search(ApplicationUser currentUser)
        {
            var viewModel = new SearchViewModel
            {
                GameDefinitions = GetAllGameDefinitionsForCurrentGamingGroup(currentUser.CurrentGamingGroupId)
            };
            return View(MVC.PlayedGame.Views.Search, viewModel);
        }

        [System.Web.Mvc.Authorize]
        [UserContext]
        [System.Web.Mvc.HttpPost]
        public virtual ActionResult Search(PlayedGamesFilterViewModel filter, ApplicationUser currentUser)
        {
            var playedGameFilter = new PlayedGameFilter
            {
                EndDateGameLastUpdated = filter.DatePlayedEnd == null ? null : filter.DatePlayedEnd.Value.ToString("yyyy-MM-dd"),
                GamingGroupId = currentUser.CurrentGamingGroupId,
                StartDateGameLastUpdated = filter.DatePlayedStart == null ? null : filter.DatePlayedStart.Value.ToString("yyyy-MM-dd"),
                GameDefinitionId = filter.GameDefinitionId
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
                GameDefinitions = GetAllGameDefinitionsForCurrentGamingGroup(currentUser.CurrentGamingGroupId),
                PlayedGames = new PlayedGamesViewModel
                {
                    PlayedGameDetailsViewModels = playedGamesDetails,
                    UserCanEdit = true,
                    GamingGroupId = currentUser.CurrentGamingGroupId,
                    ShowSearchLinkInResultsHeader = false
                }
            };
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
    }
}