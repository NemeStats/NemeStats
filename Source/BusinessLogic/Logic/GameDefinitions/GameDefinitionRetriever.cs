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
using System.Data.Entity;
using System.Net.Sockets;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Models;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic.GameDefinitions
{
    public class GameDefinitionRetriever : IGameDefinitionRetriever
    {
        private readonly IDataContext dataContext;
        private readonly IPlayerRepository playerRepository;  

        public GameDefinitionRetriever(IDataContext dataContext, IPlayerRepository playerRepository)
        {
            this.dataContext = dataContext;
            this.playerRepository = playerRepository;
        }

        public virtual IList<GameDefinitionSummary> GetAllGameDefinitions(int gamingGroupId)
        {
            var returnValue = (from gameDefinition in dataContext.GetQueryable<GameDefinition>()
                where gameDefinition.GamingGroupId == gamingGroupId
                        && gameDefinition.Active
                select new GameDefinitionSummary
                {
                    Active = gameDefinition.Active,
                    BoardGameGeekObjectId = gameDefinition.BoardGameGeekObjectId,
                    Name = gameDefinition.Name,
                    Description = gameDefinition.Description,
                    GamingGroupId = gameDefinition.GamingGroupId,
                    Id = gameDefinition.Id,
                    PlayedGames = gameDefinition.PlayedGames,
                    TotalNumberOfGamesPlayed = gameDefinition.PlayedGames.Count,
                    Champion = gameDefinition.Champion,
                    ChampionId = gameDefinition.ChampionId,
                    PreviousChampion = gameDefinition.PreviousChampion,
                    PreviousChampionId = gameDefinition.PreviousChampionId,
                    DateCreated = gameDefinition.DateCreated
                })
                .OrderBy(game => game.Name)
                .ToList();

            AddPlayersToChampionData(returnValue);

            returnValue.ForEach( summary =>
            {
                summary.BoardGameGeekUri = BoardGameGeekUriBuilder.BuildBoardGameGeekGameUri(summary.BoardGameGeekObjectId);
                summary.Champion = summary.Champion ?? new NullChampion();
                summary.PreviousChampion = summary.PreviousChampion ?? new NullChampion();
            });
            return returnValue;
        }

        private void AddPlayersToChampionData(List<GameDefinitionSummary> gameDefinitionSummaries)
        {
            var playerIds = gameDefinitionSummaries.Select(x => x.Champion == null ? -1 : x.Champion.PlayerId).ToList()
                                                   .Union(gameDefinitionSummaries.Select(x => x.PreviousChampion == null ? -1 : x.PreviousChampion.PlayerId).ToList());

            var players = dataContext.GetQueryable<Player>().Where(player => playerIds.Contains(player.Id)).ToList();

            foreach (var gameDefinitionSummary in gameDefinitionSummaries)
            {
                if (gameDefinitionSummary.Champion != null)
                {
                    gameDefinitionSummary.Champion.Player = players.FirstOrDefault(player => player.Id == gameDefinitionSummary.Champion.PlayerId);
                }

                if (gameDefinitionSummary.PreviousChampion != null)
                {
                    gameDefinitionSummary.PreviousChampion.Player = players.FirstOrDefault(player => player.Id == gameDefinitionSummary.PreviousChampion.PlayerId);
                }
            }
        }

        public virtual GameDefinitionSummary GetGameDefinitionDetails(int id, int numberOfPlayedGamesToRetrieve)
        {
            GameDefinition gameDefinition = dataContext.GetQueryable<GameDefinition>()
                .Include(game => game.PlayedGames)
                .Include(game => game.Champion)
                .Include(game => game.Champion.Player)
                .Include(game => game.PreviousChampion)
                .Include(game => game.PreviousChampion.Player)
                .Include(game => game.GamingGroup)
                .SingleOrDefault(game => game.Id == id);

            GameDefinitionSummary gameDefinitionSummary =  new GameDefinitionSummary
                                                           {
                                                               Active = gameDefinition.Active,
                                                               BoardGameGeekObjectId = gameDefinition.BoardGameGeekObjectId,
                                                               BoardGameGeekUri = BoardGameGeekUriBuilder.BuildBoardGameGeekGameUri(gameDefinition.BoardGameGeekObjectId),
                                                               Name = gameDefinition.Name,
                                                               Description = gameDefinition.Description,
                                                               GamingGroup = gameDefinition.GamingGroup,
                                                               GamingGroupId = gameDefinition.GamingGroupId,
                                                               GamingGroupName = gameDefinition.GamingGroup.Name,
                                                               Id = gameDefinition.Id,
                                                               TotalNumberOfGamesPlayed = gameDefinition.PlayedGames.Count,
                                                               Champion = gameDefinition.Champion ?? new NullChampion(),
                                                               PreviousChampion = gameDefinition.PreviousChampion ?? new NullChampion()
                                                           };

            IList<PlayedGame> playedGames = AddPlayedGamesToTheGameDefinition(numberOfPlayedGamesToRetrieve, gameDefinitionSummary);
            IList<int> distinctPlayerIds = AddPlayerGameResultsToEachPlayedGame(playedGames);
            AddPlayersToPlayerGameResults(playedGames, distinctPlayerIds);
            gameDefinitionSummary.PlayerWinRecords = playerRepository.GetPlayerWinRecords(id);

            return gameDefinitionSummary;
        }

        private IList<PlayedGame> AddPlayedGamesToTheGameDefinition(
            int numberOfPlayedGamesToRetrieve,
            GameDefinitionSummary gameDefinitionSummary)
        {
            IList<PlayedGame> playedGames = dataContext.GetQueryable<PlayedGame>().Include(playedGame => playedGame.PlayerGameResults)
                .Where(playedGame => playedGame.GameDefinitionId == gameDefinitionSummary.Id)
                .OrderByDescending(playedGame => playedGame.DatePlayed)
                .Take(numberOfPlayedGamesToRetrieve)
                .ToList();

            foreach (PlayedGame playedGame in playedGames)
            {
                playedGame.GameDefinition = gameDefinitionSummary;
            }

            gameDefinitionSummary.PlayedGames = playedGames;

            return playedGames;
        }

        private IList<int> AddPlayerGameResultsToEachPlayedGame(IList<PlayedGame> playedGames)
        {
            List<int> playedGameIds = (from playedGame in playedGames
                                       select playedGame.Id).ToList();

            IList<PlayerGameResult> playerGameResults = dataContext.GetQueryable<PlayerGameResult>()
                .Where(playerGameResult => playedGameIds.Contains(playerGameResult.PlayedGameId))
                .OrderBy(playerGameResult => playerGameResult.GameRank)
                .ToList();

            HashSet<int> distinctPlayerIds = new HashSet<int>();

            foreach (PlayedGame playedGame in playedGames)
            {
                playedGame.PlayerGameResults = (from playerGameResult in playerGameResults
                                                where playerGameResult.PlayedGameId == playedGame.Id
                                                select playerGameResult).ToList();

                ExtractDistinctListOfPlayerIds(distinctPlayerIds, playedGame);
            }

            return distinctPlayerIds.ToList();
        }

        private static void ExtractDistinctListOfPlayerIds(HashSet<int> distinctPlayerIds, PlayedGame playedGame)
        {
            foreach (PlayerGameResult playerGameResult in playedGame.PlayerGameResults)
            {
                if (!distinctPlayerIds.Contains(playerGameResult.PlayerId))
                {
                    distinctPlayerIds.Add(playerGameResult.PlayerId);
                }
            }
        }

        private void AddPlayersToPlayerGameResults(IList<PlayedGame> playedGames, IList<int> distinctPlayerIds)
        {
            IList<Player> players = dataContext.GetQueryable<Player>()
                .Where(player => distinctPlayerIds.Contains(player.Id))
                .ToList();

            foreach (PlayedGame playedGame in playedGames)
            {
                foreach (PlayerGameResult playerGameResult in playedGame.PlayerGameResults)
                {
                    playerGameResult.Player = players.First(player => player.Id == playerGameResult.PlayerId);
                }
            }
        }


        public IList<GameDefinitionName> GetAllGameDefinitionNames(Models.User.ApplicationUser currentUser)
        {
            return dataContext.GetQueryable<GameDefinition>()
                              .Where(gameDefinition => gameDefinition.Active
                              && gameDefinition.GamingGroupId == currentUser.CurrentGamingGroupId)
                              .Select(gameDefiniton => new GameDefinitionName
                              {
                                  Id = gameDefiniton.Id,
                                  Name = gameDefiniton.Name
                              }).ToList();
        }
    }
}
