using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameGeekApiClient.Interfaces;
using BoardGameGeekApiClient.Models;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using Microsoft.AspNet.SignalR;
using NemeStats.Hubs;

namespace BusinessLogic.Logic.BoardGameGeek
{
    public class BoardGameGeekGamesImporter : IBoardGameGeekGamesImporter
    {
        private readonly IUserRetriever _userRetriever;
        private readonly IBoardGameGeekApiClient _boardGameGeekApiClient;
        private readonly IGameDefinitionRetriever _gameDefinitionRetriever;
        private readonly IGameDefinitionSaver _gameDefinitionSaver;

        public BoardGameGeekGamesImporter(IUserRetriever userRetriever, IBoardGameGeekApiClient boardGameGeekApiClient, IGameDefinitionRetriever gameDefinitionRetriever, IGameDefinitionSaver gameDefinitionSaver)
        {
            _userRetriever = userRetriever;
            _boardGameGeekApiClient = boardGameGeekApiClient;
            _gameDefinitionRetriever = gameDefinitionRetriever;
            _gameDefinitionSaver = gameDefinitionSaver;
        }

        public int? ImportBoardGameGeekGames(ApplicationUser applicationUser)
        {
            if (applicationUser == null)
            {
                throw new ArgumentNullException();
            }
            var bggUser = _userRetriever.RetrieveUserInformation(applicationUser)?.BoardGameGeekUser;
            if (bggUser != null)
            {
                var userGames = _boardGameGeekApiClient.GetUserGames(bggUser.Name);
                if (!userGames.Any())
                {
                    return null;
                }
                else
                {
                    var currentGames = GetCurrentGames(applicationUser);
                    var pendingGames = GetPendingGames(userGames, currentGames);
                    if (!pendingGames.Any())
                    {
                        return 0;
                    }
                    else
                    {
                        var longRunningClients = GlobalHost.ConnectionManager.GetHubContext<LongRunningTaskHub>().Clients.Group(applicationUser.CurrentGamingGroupId.ToString());

                        int gamesImported = 0;
                        var gameNamesImported = new List<string>();
                        
                        foreach (var bggGame in pendingGames)
                        {
                            var gameName = $"{bggGame.Name} ({bggGame.YearPublished})";
                            gamesImported++;

                            longRunningClients.BGGImportDetailsProgress(gamesImported, pendingGames.Count, gameName);

                            if (!gameNamesImported.Contains(gameName))
                            {

                                _gameDefinitionSaver.CreateGameDefinition(new CreateGameDefinitionRequest()
                                {
                                    Name = $"{bggGame.Name} ({bggGame.YearPublished})",
                                    BoardGameGeekGameDefinitionId = bggGame.GameId,
                                    Active = true
                                }, applicationUser);
                            }


                            gameNamesImported.Add(gameName);
                            
                        }
                        return pendingGames.Count;
                    }
                }
            }
            return null;
        }

        private static List<GameDetails> GetPendingGames(List<GameDetails> userGames, IEnumerable<int?> currentGames)
        {
            var pendingGames = userGames.Where(g => currentGames.All(id => g.GameId != id)).ToList();
            return pendingGames;
        }

        private IEnumerable<int?> GetCurrentGames(ApplicationUser applicationUser)
        {
            var currentGames =
                _gameDefinitionRetriever.GetAllGameDefinitionNames(applicationUser.CurrentGamingGroupId)
                    .Select(cg => cg.BoardGameGeekGameDefinitionId);
            return currentGames;
        }
    }
}
