using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BoardGameGeekApiClient.Interfaces;
using BoardGameGeekApiClient.Models;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.BoardGameGeek
{
    public class BoardGameGeekCleanUpService : IBoardGameGeekCleanUpService
    {
        private const string CleanYearPattern = @"\w*\(\d{4}\)";

        private readonly IDataContext _dataContext;
        private readonly IBoardGameGeekApiClient _boardGameGeekApiClient;

        public BoardGameGeekCleanUpService(IDataContext dataContext, IBoardGameGeekApiClient boardGameGeekApiClient)
        {
            this._dataContext = dataContext;
            _boardGameGeekApiClient = boardGameGeekApiClient;
        }

        public void LinkGameDefinitionsWithBGG()
        {
            var dirtyGames = GetDirtyGames();

            foreach (var game in dirtyGames)
            {
                var gameName = RemoveYear(game.Name);

                var existingGame = GetExistingBGGGameByName(gameName);
                if (existingGame != null)
                {
                    game.BoardGameGeekGameDefinitionId = existingGame.Id;
                    _dataContext.CommitAllChanges();
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
                                game.BoardGameGeekGameDefinitionId = existingGame.Id;
                            }
                            else
                            {
                                var newRecord = CreateBGGGame(gameToAdd);

                                game.BoardGameGeekGameDefinitionId = newRecord.Id;
                            }

                            _dataContext.CommitAllChanges();
                        }
                    }
                }
            }
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