using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.GameDefinitions
{
    public class GameDefinitionCreatorImpl : GameDefinitionCreator
    {
        //TODO add event tracking for newly created game definitions
        private DataContext dataContext;

        public GameDefinitionCreatorImpl(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public GameDefinition CreateGameDefinition(string gameDefinitionName, ApplicationUser currentUser)
        {
            ValidateGameDefinitionNameIsNotNullOrWhitespace(gameDefinitionName);

            GameDefinition gameDefinition = new GameDefinition()
            {
                Name = gameDefinitionName
            };

            return dataContext.Save<GameDefinition>(gameDefinition, currentUser);
        }

        private static void ValidateGameDefinitionNameIsNotNullOrWhitespace(string gameDefinitionName)
        {
            if (string.IsNullOrWhiteSpace(gameDefinitionName))
            {
                throw new ArgumentNullException("gameDefinitionName");
            }
        }
    }
}
