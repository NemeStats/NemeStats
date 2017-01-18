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

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.Champions;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Champions
{

    public class ChampionRecalculator : IChampionRecalculator
    {
        private readonly IDataContext _dataContext;
        private readonly IChampionRepository _championRepository;

        public ChampionRecalculator(IDataContext dataContext, IChampionRepository championRepository)
        {
            this._dataContext = dataContext;
            this._championRepository = championRepository;
        }

        public void RecalculateAllChampions()
        {
            ApplicationUser applicationUser = new ApplicationUser();

            List<GameDefinition> gameDefinitions =
                _dataContext.GetQueryable<GameDefinition>()
                    .Where(definition => definition.Active)
                    .ToList();

            foreach (GameDefinition gameDefinition in gameDefinitions)
            {
                applicationUser.CurrentGamingGroupId = gameDefinition.GamingGroupId;
                RecalculateChampion(gameDefinition.Id, applicationUser, _dataContext);
            }
        }

        public virtual Champion RecalculateChampion(int gameDefinitionId, ApplicationUser applicationUser, IDataContext toClearExistingChampion, bool allowedToClearExistingChampion = true)
        {
            GameDefinition gameDefinition = _dataContext.FindById<GameDefinition>(gameDefinitionId);

            ChampionData championData = _championRepository.GetChampionData(gameDefinitionId);

            if (championData is NullChampionData)
            {
                if (allowedToClearExistingChampion)
                {
                    ClearChampionId(applicationUser, gameDefinition);
                }

                return new NullChampion();
            }

            Champion existingChampion =
                _dataContext.GetQueryable<Champion>()
                    .Include(champion => champion.GameDefinition)
                .FirstOrDefault(champion => champion.GameDefinitionId == gameDefinitionId && champion.GameDefinition.ChampionId == champion.Id);

            Champion newChampion = new Champion
            {
                WinPercentage = championData.WinPercentage,
                NumberOfGames = championData.NumberOfGames,
                NumberOfWins = championData.NumberOfWins,
                GameDefinitionId = gameDefinitionId,
                PlayerId = championData.PlayerId
            };

            Champion savedChampion;

            if (newChampion.SameChampion(existingChampion))
            {
                savedChampion = UpdateExistingChampionIfNecessary(applicationUser, existingChampion, newChampion);
            }
            else
            {
                savedChampion = _dataContext.Save(newChampion, applicationUser);
                _dataContext.CommitAllChanges();
                gameDefinition.PreviousChampionId = gameDefinition.ChampionId;
                gameDefinition.ChampionId = savedChampion.Id;
                _dataContext.Save(gameDefinition, applicationUser);
            }

            return savedChampion;
        }

        private void ClearChampionId(ApplicationUser applicationUser, GameDefinition gameDefinition)
        {
            if (gameDefinition.ChampionId != null)
            {
                gameDefinition.PreviousChampionId = gameDefinition.ChampionId;
                gameDefinition.ChampionId = null;
                _dataContext.Save(gameDefinition, applicationUser);
            }
        }

        private Champion UpdateExistingChampionIfNecessary(ApplicationUser applicationUser, Champion existingChampion, Champion newChampion)
        {
            if (!newChampion.Equals(existingChampion))
            {
                existingChampion.WinPercentage = newChampion.WinPercentage;
                existingChampion.NumberOfGames = newChampion.NumberOfGames;
                existingChampion.NumberOfWins = newChampion.NumberOfWins;
                Champion returnChampion = _dataContext.Save(existingChampion, applicationUser);
                _dataContext.CommitAllChanges();
                return returnChampion;
            }
            return existingChampion;
        }
    }
}
