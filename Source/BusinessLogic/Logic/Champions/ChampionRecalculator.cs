using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.Champions;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Champions
{

    public class ChampionRecalculator : IChampionRecalculator
    {
        private readonly IDataContext dataContext;
        private readonly IChampionRepository championRepository;

        public ChampionRecalculator(IDataContext dataContext, IChampionRepository championRepository)
        {
            this.dataContext = dataContext;
            this.championRepository = championRepository;
        }

        public void RecalculateAllChampions()
        {
            ApplicationUser applicationUser = new ApplicationUser();

            List<GameDefinition> gameDefinitions =
                dataContext.GetQueryable<GameDefinition>()
                    .Where(definition => definition.Active)
                    .ToList();

            foreach (GameDefinition gameDefinition in gameDefinitions)
            {
                applicationUser.CurrentGamingGroupId = gameDefinition.GamingGroupId;
                RecalculateChampion(gameDefinition.Id, applicationUser);
            }
        }

        public virtual Champion RecalculateChampion(int gameDefinitionId, ApplicationUser applicationUser)
        {
            GameDefinition gameDefinition = dataContext.FindById<GameDefinition>(gameDefinitionId);

            ChampionData championData = championRepository.GetChampionData(gameDefinitionId);

            if (championData is NullChampionData)
            {
                ClearChampionId(applicationUser, gameDefinition);

                return new NullChampion();
            }

            Champion existingChampion =
                dataContext.GetQueryable<Champion>()
                    .Where(champion => champion.GameDefinitionId == gameDefinitionId)
                    .FirstOrDefault();

            Champion newChampion = new Champion
            {
                WinPercentage = championData.WinPercentage,
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
                savedChampion = dataContext.Save(newChampion, applicationUser);
                dataContext.CommitAllChanges();
                gameDefinition.PreviousChampionId = gameDefinition.ChampionId;
                gameDefinition.ChampionId = savedChampion.Id;
                dataContext.Save(gameDefinition, applicationUser);
            }

            return savedChampion;
        }

        private void ClearChampionId(ApplicationUser applicationUser, GameDefinition gameDefinition)
        {
            if (gameDefinition.ChampionId != null)
            {
                gameDefinition.PreviousChampionId = gameDefinition.ChampionId;
                gameDefinition.ChampionId = null;
                dataContext.Save(gameDefinition, applicationUser);
            }
        }

        private Champion UpdateExistingChampionIfNecessary(ApplicationUser applicationUser, Champion existingChampion, Champion newChampion)
        {
            if (!newChampion.Equals(existingChampion))
            {
                existingChampion.WinPercentage = newChampion.WinPercentage;
                Champion returnChampion = dataContext.Save(existingChampion, applicationUser);
                dataContext.CommitAllChanges();
                return returnChampion;
            }
            return existingChampion;
        }
    }
}
