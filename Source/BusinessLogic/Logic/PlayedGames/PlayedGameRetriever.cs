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
using System.Security.Cryptography.X509Certificates;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Logic.PlayedGames
{
    public class PlayedGameRetriever : IPlayedGameRetriever
    {
        private readonly IDataContext dataContext;

        public PlayedGameRetriever(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public List<PlayedGame> GetRecentGames(int numberOfGames, int gamingGroupId)
        {
            List<PlayedGame> playedGames = dataContext.GetQueryable<PlayedGame>()
                .Where(game => game.GamingGroupId == gamingGroupId)
                .Include(playedGame => playedGame.GameDefinition)
                .Include(playedGame => playedGame.GamingGroup)
                .Include(playedGame => playedGame.PlayerGameResults
                    .Select(playerGameResult => playerGameResult.Player))
                    .OrderByDescending(orderBy => orderBy.DatePlayed)
                    .ThenByDescending(orderBy => orderBy.DateCreated)
                .Take(numberOfGames)
                .ToList();

            //TODO this seems ridiculous but I can't see how to order a related entity in Entity Framework :(
            foreach (PlayedGame playedGame in playedGames)
            {
                playedGame.PlayerGameResults = playedGame.PlayerGameResults.OrderBy(orderBy => orderBy.GameRank).ToList();
            }

            return playedGames;
        }


        public PlayedGame GetPlayedGameDetails(int playedGameId)
        {
            PlayedGame result = dataContext.GetQueryable<PlayedGame>()
                .Where(playedGame => playedGame.Id == playedGameId)
                    .Include(playedGame => playedGame.GameDefinition)
                    .Include(playedGame => playedGame.GamingGroup)
                    .Include(playedGame => playedGame.PlayerGameResults
                        .Select(playerGameResult => playerGameResult.Player))
                    .FirstOrDefault();

            if (result == null)
            {
                throw new EntityDoesNotExistException(playedGameId);
            }

            result.PlayerGameResults = result.PlayerGameResults.OrderBy(playerGameResult => playerGameResult.GameRank).ToList();

            return result;
        }

        public List<PublicGameSummary> GetRecentPublicGames(int numberOfGames)
        {
            return (from playedGame in dataContext.GetQueryable<PlayedGame>()
                        .OrderByDescending(game => game.DatePlayed)
                        .ThenByDescending(game => game.DateCreated)
                    select new PublicGameSummary
             {
                 PlayedGameId = playedGame.Id,
                 GameDefinitionId = playedGame.GameDefinitionId,
                 GameDefinitionName = playedGame.GameDefinition.Name,
                 GamingGroupId = playedGame.GamingGroupId,
                 GamingGroupName = playedGame.GamingGroup.Name,
                 WinnerType = (playedGame.PlayerGameResults.All(x => x.GameRank == 1) ? WinnerTypes.TeamWin :
                                playedGame.PlayerGameResults.All(x => x.GameRank != 1) ? WinnerTypes.TeamLoss :
                                WinnerTypes.PlayerWin),
                 WinningPlayer = playedGame.PlayerGameResults.FirstOrDefault(player => player.GameRank == 1).Player,
                 DatePlayed = playedGame.DatePlayed
             }).Take(numberOfGames)
                                .ToList();
        }
    }
}
