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
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Logic.PlayedGames
{
    public class PlayedGameDeleter : IPlayedGameDeleter
    {
        private readonly IDataContext dataContext;
        private readonly INemesisRecalculator nemesisRecalculator;
        private readonly IChampionRecalculator championRecalculator;

        public PlayedGameDeleter(IDataContext dataContext, INemesisRecalculator nemesisRecalculatorMock, IChampionRecalculator championRecalculator)
        {
            this.dataContext = dataContext;
            this.nemesisRecalculator = nemesisRecalculatorMock;
            this.championRecalculator = championRecalculator;
        }

        public void DeletePlayedGame(int playedGameId, ApplicationUser currentUser)
        {
            List<int> playerIds = (from playerResult in dataContext.GetQueryable<PlayerGameResult>()
                                   where playerResult.PlayedGameId == playedGameId
                                   select playerResult.PlayerId).ToList();
            var gameDefId = dataContext.GetQueryable<PlayerGameResult>()
                             .Where(p => p.PlayedGameId == playedGameId)
                             .Select(p => p.PlayedGame.GameDefinitionId)
                             .FirstOrDefault();

            dataContext.DeleteById<PlayedGame>(playedGameId, currentUser);
            dataContext.CommitAllChanges();

            foreach (int playerId in playerIds)
            {
                nemesisRecalculator.RecalculateNemesis(playerId, currentUser, dataContext);
            }

            championRecalculator.RecalculateChampion(gameDefId, currentUser, dataContext);
        }
    }
}
