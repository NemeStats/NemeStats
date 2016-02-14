using System;
using System.Linq;
using BoardGameGeekApiClient.Interfaces;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using UniversalAnalyticsHttpWrapper;

namespace BusinessLogic.Logic.BoardGameGeek
{
    public class BoardGameGeekGamesImporter: IBoardGameGeekGamesImporter
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
                    var currentGames =
                        _gameDefinitionRetriever.GetAllGameDefinitionNames(applicationUser.CurrentGamingGroupId)
                            .Select(cg => cg.BoardGameGeekGameDefinitionId);


                    var pendingGames = userGames.Where(g => currentGames.All(id => g.GameId != id)).ToList();
                    if (!pendingGames.Any())
                    {
                        return 0;
                    }
                    else
                    {

                        foreach (var bggGame in pendingGames)
                        {
                            _gameDefinitionSaver.CreateGameDefinition(new CreateGameDefinitionRequest()
                            {
                                Name = $"{bggGame.Name} ({bggGame.YearPublished})",
                                BoardGameGeekGameDefinitionId = bggGame.GameId,
                                Active = true
                            }, applicationUser);
                        }
                        return pendingGames.Count;
                    }
                }
            }
            return null;
        }
    }
}
