using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Logic.Players
{
    public class PlayerCreatorImpl : PlayerCreator
    {
        //TODO add event tracking for newly created game definitions
        private DataContext dataContext;

        public PlayerCreatorImpl(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public Player CreatePlayer(string playerName, ApplicationUser currentUser)
        {
            ValidatePlayerNameIsNotNullOrWhiteSpace(playerName);

            Player player = new Player()
            {
                Name = playerName
            };

            return dataContext.Save<Player>(player, currentUser);
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
