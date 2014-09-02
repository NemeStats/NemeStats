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
    public class PlayerCreatorImpl : PlayerCreator
    {
        private DataContext dataContext;
        private NemeStatsEventTracker eventTracker;

        public PlayerCreatorImpl(DataContext dataContext, NemeStatsEventTracker eventTracker)
        {
            this.dataContext = dataContext;
            this.eventTracker = eventTracker;
        }

        public Player CreatePlayer(string playerName, ApplicationUser currentUser)
        {
            ValidatePlayerNameIsNotNullOrWhiteSpace(playerName);

            Player player = new Player()
            {
                Name = playerName
            };

            Player newPlayer = dataContext.Save<Player>(player, currentUser);

            new Task(() => eventTracker.TrackPlayerCreation(currentUser)).Start();

            return newPlayer;
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
