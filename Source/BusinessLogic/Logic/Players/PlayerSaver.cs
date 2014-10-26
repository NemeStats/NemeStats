using System.Data.Entity.Infrastructure;
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Nemeses;

namespace BusinessLogic.Logic.Players
{
    public class PlayerSaver : IPlayerSaver
    {
        private readonly IDataContext dataContext;
        private readonly INemeStatsEventTracker eventTracker;
        private readonly INemesisRecalculator nemesisRecalculator;

        public PlayerSaver(IDataContext dataContext, INemeStatsEventTracker eventTracker, INemesisRecalculator nemesisRecalculator)
        {
            this.dataContext = dataContext;
            this.eventTracker = eventTracker;
            this.nemesisRecalculator = nemesisRecalculator;
        }

        public Player Save(Player player, ApplicationUser currentUser)
        {
            ValidatePlayerIsNotNull(player);
            ValidatePlayerNameIsNotNullOrWhiteSpace(player.Name);
            ValidatePlayerWithThisNameDoesntAlreadyExist(player, currentUser);

            Player newPlayer = dataContext.Save<Player>(player, currentUser);
            dataContext.CommitAllChanges();

            if (!player.AlreadyInDatabase())
            {
                new Task(() => eventTracker.TrackPlayerCreation(currentUser)).Start();
            }else
            {
                if(!player.Active)
                {
                    this.RecalculateNemeses(player, currentUser);
                }
            }

            return newPlayer;
        }

        private void ValidatePlayerWithThisNameDoesntAlreadyExist(Player player, ApplicationUser currentUser)
        {
            if (!player.AlreadyInDatabase())
            {
                Player existingPlayerWithThisName = this.dataContext.GetQueryable<Player>().FirstOrDefault(
                                                                                       p => p.GamingGroupId == currentUser.CurrentGamingGroupId
                                                                                            && p.Name == player.Name);

                if (existingPlayerWithThisName != null)
                {
                    throw new PlayerAlreadyExistsException(existingPlayerWithThisName.Id);
                } 
            }
        }

        private void RecalculateNemeses(Player player, ApplicationUser currentUser)
        {
            List<int> playerIdsToRecalculate = (from thePlayer in this.dataContext.GetQueryable<Player>()
                                                where thePlayer.Active == true
                                                      && thePlayer.Nemesis.NemesisPlayerId == player.Id
                                                select thePlayer.Id).ToList();

            foreach (int playerId in playerIdsToRecalculate)
            {
                this.nemesisRecalculator.RecalculateNemesis(playerId, currentUser);
            }
        }

        private static void ValidatePlayerIsNotNull(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
        }

        private static void ValidatePlayerNameIsNotNullOrWhiteSpace(string playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                throw new ArgumentNullException("playerName");
            }
        }
    }
}
