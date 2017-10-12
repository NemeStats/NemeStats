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
        private readonly IDataContext _dataContext;
        private readonly INemesisRecalculator _nemesisRecalculator;
        private readonly IChampionRecalculator _championRecalculator;

        public PlayedGameDeleter(IDataContext dataContext, INemesisRecalculator nemesisRecalculatorMock, IChampionRecalculator championRecalculator)
        {
            this._dataContext = dataContext;
            this._nemesisRecalculator = nemesisRecalculatorMock;
            this._championRecalculator = championRecalculator;
        }

        public void DeletePlayedGame(int playedGameId, ApplicationUser currentUser)
        {
            List<int> playerIds = (from playerResult in _dataContext.GetQueryable<PlayerGameResult>()
                                   where playerResult.PlayedGameId == playedGameId
                                   select playerResult.PlayerId).ToList();
            var gameDefId = _dataContext.GetQueryable<PlayerGameResult>()
                             .Where(p => p.PlayedGameId == playedGameId)
                             .Select(p => p.PlayedGame.GameDefinitionId)
                             .FirstOrDefault();

            _dataContext.DeleteById<PlayedGame>(playedGameId, currentUser);
            _dataContext.CommitAllChanges();

            foreach (int playerId in playerIds)
            {
                _nemesisRecalculator.RecalculateNemesis(playerId, currentUser, _dataContext);
            }

            _championRecalculator.RecalculateChampion(gameDefId, currentUser, _dataContext);
        }
    }
}
