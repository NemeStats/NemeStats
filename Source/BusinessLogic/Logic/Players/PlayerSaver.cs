using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.Players
{
    public class PlayerSaver : IPlayerSaver
    {
        private DataContext dataContext;
        private NemeStatsEventTracker eventTracker;

        public PlayerSaver(DataContext dataContext, NemeStatsEventTracker eventTracker)
        {
            this.dataContext = dataContext;
            this.eventTracker = eventTracker;
        }

        public Player Save(Player player, ApplicationUser currentUser)
        {
            ValidatePlayerIsNotNull(player);
            ValidatePlayerNameIsNotNullOrWhiteSpace(player.Name);

            bool isNewPlayer = !player.AlreadyInDatabase();

            Player newPlayer = dataContext.Save<Player>(player, currentUser);

            if(isNewPlayer)
            {
                new Task(() => eventTracker.TrackPlayerCreation(currentUser)).Start();
            }

            return newPlayer;
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
