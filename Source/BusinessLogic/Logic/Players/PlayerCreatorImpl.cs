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
        private DataContext dataContext;

        public PlayerCreatorImpl(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public Player CreatePlayer(string playerName, ApplicationUser currentUser)
        {
            if(string.IsNullOrWhiteSpace(playerName))
            {
                throw new ArgumentNullException("playerName");
            }

            Player player = new Player()
            {
                Name = playerName
            };

            return dataContext.Save<Player>(player, currentUser);
        }
    }
}
