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

namespace BusinessLogic.Logic.BoardGameGeek
{
    public class BoardGameGeekCleanUpResult
    {
        public BoardGameGeekCleanUpResult()
        {
            CleanedGames = 0;
            Success = true;
            UncleanableGames = new List<UncleanableGame>();
        }

        public int DirtyGames { get; set; }
        public int CleanedGames { get; set; }

        public List<UncleanableGame> UncleanableGames { get; set; }
        public TimeSpan TimeEllapsed { get; set; }
        public bool Success { get; set; }

        public class UncleanableGame
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public int GamingGroupId { get; set; }
        }
    }
    public class BoardGameGeekCleanUpService : IBoardGameGeekCleanUpService
    {
        private const string CleanYearPattern = @"\w*\(\d{4}\)";

        private readonly IDataContext _dataContext;
        private readonly IBoardGameGeekApiClient _boardGameGeekApiClient;
        private readonly RollbarClient _rollbar;

        public BoardGameGeekCleanUpService(IDataContext dataContext, IBoardGameGeekApiClient boardGameGeekApiClient)
        {
            this._dataContext = dataContext;
            _boardGameGeekApiClient = boardGameGeekApiClient;
            _rollbar = new RollbarClient();
        }

        public BoardGameGeekCleanUpResult LinkGameDefinitionsWithBGG()
        {
            var result = new BoardGameGeekCleanUpResult();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {


                var dirtyGames = GetDirtyGames();
                result.DirtyGames = dirtyGames.Count;

                foreach (var game in dirtyGames)
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
                        result.UncleanableGames.Add(new BoardGameGeekCleanUpResult.UncleanableGame()
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
            BoardGameGeekCleanUpResult result)
        {
            game.BoardGameGeekGameDefinitionId = boardGameGeekGameDefinitionId;
            result.CleanedGames++;
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
                PlayingTime = gameToAdd.PlayingTime,
                AverageWeight = gameToAdd.AverageWeight,
                Description = gameToAdd.Description
            };

            _dataContext.Save(newRecord, new ApplicationUser());
            return newRecord;
        }

        private BoardGameGeekGameDefinition GetExistingBGGGameById(GameDetails gameToAdd)
        {
            BoardGameGeekGameDefinition existingGame;
            existingGame =
                _dataContext.GetQueryable<BoardGameGeekGameDefinition>()
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

        private List<GameDefinition> GetDirtyGames()
        {
            var gamesWithoutBGGLink =
                _dataContext.GetQueryable<GameDefinition>().Where(g => g.BoardGameGeekGameDefinitionId == null).ToList();
            return gamesWithoutBGGLink;
        }

        private string RemoveYear(string name)
        {
            return Regex.Replace(name, CleanYearPattern, "");
        }
    }
}