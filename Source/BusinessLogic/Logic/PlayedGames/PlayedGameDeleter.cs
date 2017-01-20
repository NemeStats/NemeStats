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
using BusinessLogic.Logic.MVP;

namespace BusinessLogic.Logic.PlayedGames
{
    public class PlayedGameDeleter : IPlayedGameDeleter
    {
        private readonly IDataContext _dataContext;
        private readonly INemesisRecalculator _nemesisRecalculator;
        private readonly IChampionRecalculator _championRecalculator;
        private readonly IMVPRecalculator _mvpRecalculator;

        public PlayedGameDeleter(IDataContext dataContext, INemesisRecalculator nemesisRecalculatorMock, IChampionRecalculator championRecalculator, IMVPRecalculator mvpRecalculator)
        {
            this._dataContext = dataContext;
            this._nemesisRecalculator = nemesisRecalculatorMock;
            this._championRecalculator = championRecalculator;
            _mvpRecalculator = mvpRecalculator;
        }

        public void DeletePlayedGame(int playedGameId, ApplicationUser currentUser)
        {
            var playedGameResults = (from playerResult in _dataContext.GetQueryable<PlayerGameResult>()
                                   where playerResult.PlayedGameId == playedGameId
                                   select playerResult).ToList();
            var gameDefId = _dataContext.GetQueryable<PlayerGameResult>()
                             .Where(p => p.PlayedGameId == playedGameId)
                             .Select(p => p.PlayedGame.GameDefinitionId)
                             .FirstOrDefault();

            foreach (var playedGameResult in playedGameResults)
            {

                var mvp = _dataContext.GetQueryable<Models.MVP>().FirstOrDefault(m => m.PlayedGameResultId == playedGameResult.Id);

                if (mvp != null)
                {
                    var gameDefinition = _dataContext.FindById<GameDefinition>(gameDefId);
                    gameDefinition.MVPId = gameDefinition.PreviousMVPId;
                    gameDefinition.PreviousMVPId = null;

                    _dataContext.DeleteById<Models.MVP>(mvp.Id, currentUser);

                    _dataContext.CommitAllChanges();
                }

            }

            

            _dataContext.DeleteById<PlayedGame>(playedGameId, currentUser);
            _dataContext.CommitAllChanges();

            foreach (int playerId in playedGameResults.Select(pgr=>pgr.PlayerId).ToList())
            {
                _nemesisRecalculator.RecalculateNemesis(playerId, currentUser);
            }

            _championRecalculator.RecalculateChampion(gameDefId, currentUser);
            _mvpRecalculator.RecalculateMVP(gameDefId, currentUser);
        }
    }
}
