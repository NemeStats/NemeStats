using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
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
        private DataContext dataContext;
        private NemeStatsEventTracker eventTracker;

        public GameDefinitionCreatorImpl(DataContext dataContext, NemeStatsEventTracker eventTracker)
        {
            this.dataContext = dataContext;
            this.eventTracker = eventTracker;
        }

        public GameDefinition CreateGameDefinition(string gameDefinitionName, string gameDefinitionDescription, ApplicationUser currentUser)
        {
            ValidateGameDefinitionNameIsNotNullOrWhitespace(gameDefinitionName);

            GameDefinition gameDefinition = new GameDefinition()
            {
                Name = gameDefinitionName,
                Description = gameDefinitionDescription
            };

            GameDefinition newGameDefinition = dataContext.Save<GameDefinition>(gameDefinition, currentUser);

            new Task(() => eventTracker.TrackGameDefinitionCreation(currentUser, gameDefinitionName)).Start();

            return newGameDefinition;
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
