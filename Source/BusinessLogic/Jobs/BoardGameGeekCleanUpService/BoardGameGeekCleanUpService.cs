using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using BoardGameGeekApiClient.Interfaces;
using BoardGameGeekApiClient.Models;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using RollbarSharp;

namespace BusinessLogic.Jobs.BoardGameGeekCleanUpService
{
    public class BoardGameGeekCleanUpService : IBoardGameGeekCleanUpService
    {
        private const string CleanYearPattern = @"\w*\(\d{4}\)";

        private readonly IDataContext _dataContext;
        private readonly IBoardGameGeekApiClient _boardGameGeekApiClient;
        private readonly IRollbarClient _rollbar;

        public BoardGameGeekCleanUpService(IDataContext dataContext, IBoardGameGeekApiClient boardGameGeekApiClient, IRollbarClient rollbar)
        {
            _dataContext = dataContext;
            _boardGameGeekApiClient = boardGameGeekApiClient;
            _rollbar = rollbar;
        }

        public LinkOrphanGamesResult LinkOrphanGames()
        {
            var result = new LinkOrphanGamesResult();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {


                var orphanGames = GetOrphanGames();
                result.OrphanGames = orphanGames.Count;

                foreach (var game in orphanGames)
                {
                    var gameName = RemoveYear(game.Name);

                    var existingGame = GetExistingBGGGameByName(gameName);
                    if (existingGame != null)
                    {
                        UpdateGameDefinition(game, existingGame.Id, result);
                    }
                    else
                    {
                        var searchResult = _boardGameGeekApiClient.SearchBoardGames(gameName, true);
                        if (searchResult.Any())
                        {
                            var gameToAdd = GetGameToAddFromSearch(searchResult);

                            if (gameToAdd != null)
                            {

                                existingGame = GetExistingBGGGameById(gameToAdd);
                                if (existingGame != null)
                                {
                                    UpdateGameDefinition(game, existingGame.Id, result);
                                }
                                else
                                {
                                    var newRecord = CreateBGGGame(gameToAdd);

                                    UpdateGameDefinition(game, newRecord.Id, result);
                                }
                            }
                        }
                    }

                    if (game.BoardGameGeekGameDefinitionId != null)
                    {
                        _dataContext.CommitAllChanges();
                    }
                    else
                    {
                        result.StillOrphanGames.Add(new LinkOrphanGamesResult.OrphanGame()
                        {
                            Name = game.Name,
                            Id = game.Id,
                            GamingGroupId = game.GamingGroupId
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                _rollbar.SendException(ex);
            }

            stopwatch.Stop();
            result.TimeEllapsed = stopwatch.Elapsed;
            return result;
        }

        private void UpdateGameDefinition(GameDefinition game, int boardGameGeekGameDefinitionId,
            LinkOrphanGamesResult result)
        {
            game.BoardGameGeekGameDefinitionId = boardGameGeekGameDefinitionId;
            result.LinkedGames++;
        }

        private BoardGameGeekGameDefinition CreateBGGGame(GameDetails gameToAdd)
        {
            var newRecord = new BoardGameGeekGameDefinition
            {
                Id = gameToAdd.GameId,
                Name = gameToAdd.Name,
                Thumbnail = gameToAdd.Thumbnail,
                MaxPlayers = gameToAdd.MaxPlayers,
                MinPlayers = gameToAdd.MinPlayers,
                MaxPlayTime = gameToAdd.MaxPlayTime,
                MinPlayTime = gameToAdd.MinPlayTime,
                AverageWeight = gameToAdd.AverageWeight,
                Description = gameToAdd.Description
            };

            _dataContext.Save(newRecord, new ApplicationUser());
            return newRecord;
        }

        private BoardGameGeekGameDefinition GetExistingBGGGameById(GameDetails gameToAdd)
        {
            var existingGame = _dataContext.GetQueryable<BoardGameGeekGameDefinition>()
                .FirstOrDefault(bgg => bgg.Id == gameToAdd.GameId);
            return existingGame;
        }

        private GameDetails GetGameToAddFromSearch(List<SearchBoardGameResult> searchResult)
        {
            var gamesToAdd =
                searchResult.Select(
                    searchBoardGameResult => _boardGameGeekApiClient.GetGameDetails(searchBoardGameResult.BoardGameId))
                    .Where(gameDetails => gameDetails != null)
                    .ToList();

            var gameToAdd =
                gamesToAdd.Where(g => g.Thumbnail != null)
                    .OrderByDescending(g => g.BGGRating)
                    .FirstOrDefault();
            return gameToAdd;
        }

        private BoardGameGeekGameDefinition GetExistingBGGGameByName(string gameName)
        {
            var existingGame =
                _dataContext.GetQueryable<BoardGameGeekGameDefinition>().FirstOrDefault(bgg => bgg.Name.Contains(gameName));
            return existingGame;
        }

        private List<GameDefinition> GetOrphanGames()
        {
            var gamesWithoutBGGLink =
                _dataContext.GetQueryable<GameDefinition>().Where(g => g.BoardGameGeekGameDefinitionId == null).ToList();
            return gamesWithoutBGGLink;
        }

        private string RemoveYear(string name)
        {
            return Regex.Replace(name, CleanYearPattern, "").Trim();
        }
    }
}