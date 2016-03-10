using System;
using System.Linq;
using System.Text.RegularExpressions;
using BoardGameGeekApiClient.Interfaces;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;

namespace BusinessLogic.Logic.BoardGameGeek
{
    public class BoardGameGeekCleanUp : IBoardGameGeekCleanUp
    {
        private const string CleanYearPattern = @"\w*\(\d{4}\)";

        private readonly IDataContext _dataContext;
        private readonly IBoardGameGeekApiClient _boardGameGeekApiClient;

        public BoardGameGeekCleanUp(IDataContext dataContext, IBoardGameGeekApiClient boardGameGeekApiClient)
        {
            this._dataContext = dataContext;
            _boardGameGeekApiClient = boardGameGeekApiClient;
        }

        public void LinkGameDefinitionsWithBGG()
        {
            var gamesWithoutBGGLink = _dataContext.GetQueryable<GameDefinition>().Where(g => g.BoardGameGeekGameDefinitionId == null).ToList();

            foreach (var game in gamesWithoutBGGLink)
            {
                var gameName = RemoveYear(game.Name);

                var existingGame =
                    _dataContext.GetQueryable<BoardGameGeekGameDefinition>().FirstOrDefault(bgg => bgg.Name.Contains(gameName));
                if (existingGame != null)
                {
                    game.BoardGameGeekGameDefinitionId = existingGame.Id;
                    _dataContext.CommitAllChanges();
                }
                else
                {

                    var searchResult = _boardGameGeekApiClient.SearchBoardGames(gameName, true);
                    if (searchResult.Count == 1)
                    {

                        var gameDetails = _boardGameGeekApiClient.GetGameDetails(searchResult.First().BoardGameId);

                        if (gameDetails != null)
                        {
                            var newRecord = new BoardGameGeekGameDefinition
                            {
                                Id = gameDetails.GameId,
                                Name = gameDetails.Name,
                                Thumbnail = gameDetails.Thumbnail,
                                MaxPlayers = gameDetails.MaxPlayers,
                                MinPlayers = gameDetails.MinPlayers,
                                PlayingTime = gameDetails.PlayingTime,
                                AverageWeight = gameDetails.AverageWeight,
                                Description = gameDetails.Description
                            };

                            game.BoardGameGeekGameDefinitionId = newRecord.Id;

                            _dataContext.CommitAllChanges();
                        }

                    }
                }
            }
        }

        private string RemoveYear(string name)
        {
            return Regex.Replace(name, CleanYearPattern, "");
        }
    }
}