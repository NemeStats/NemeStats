using BusinessLogic.DataAccess.Security;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.DataAccess.Repositories
{
    public class EntityFrameworkGameDefinitionRepository : GameDefinitionRepository
    {
        internal const string EXCEPTION_MESSAGE_USER_DOES_NOT_HAVE_ACCESS_TO_GAME_DEFINITION 
            = "User with Id '{0} is unauthorized to access GameDefinition with Id '{1}'";

        private DataContext dataContext;
        private SecuredEntityValidatorFactory securedEntityValidatorFactory;

        public EntityFrameworkGameDefinitionRepository(DataContext dataContext, 
            SecuredEntityValidator<GameDefinition> securedEntityValidator)
        {
            this.dataContext = dataContext;
            this.securedEntityValidatorFactory = securedEntityValidator;
        }

        public virtual List<GameDefinition> GetAllGameDefinitions(ApplicationUser currentUser)
        {
            return dataContext.GetQueryable<GameDefinition>(currentUser)
                .Where(game => game.GamingGroupId == currentUser.CurrentGamingGroupId)
                .ToList();
        }
        
        public virtual GameDefinition GetGameDefinition(
            int gameDefinitionId, 
            ApplicationUser currentUser)
        {
            GameDefinition game = dataContext.FindById<GameDefinition>(gameDefinitionId, currentUser);
            ValidateGameDefinitionIsFound(gameDefinitionId, game);

            ValidateUserHasAccessToGameDefinition(currentUser, game);

            return game;
        }

        private static void ValidateGameDefinitionIsFound(int gameDefinitionId, GameDefinition game)
        {
            if (game == null)
            {
                throw new EntityDoesNotExistException(gameDefinitionId);
            }
        }

        internal virtual void ValidateUserHasAccessToGameDefinition(ApplicationUser currentUser, GameDefinition game)
        {
            if (game.GamingGroupId != currentUser.CurrentGamingGroupId)
            {
                string notAuthorizedMessage = string.Format(
                    EXCEPTION_MESSAGE_USER_DOES_NOT_HAVE_ACCESS_TO_GAME_DEFINITION,
                    currentUser.Id,
                    game.Id);
                throw new UnauthorizedAccessException(notAuthorizedMessage);
            }
        }
    }
}
