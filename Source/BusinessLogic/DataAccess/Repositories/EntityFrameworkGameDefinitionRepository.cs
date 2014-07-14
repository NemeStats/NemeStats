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
        internal const string EXCEPTION_MESSAGE_GAME_DEFINITION_NOT_FOUND
            = "Game Definition with Id '{0}' not found.";

        private NemeStatsDbContext dbContext;

        public EntityFrameworkGameDefinitionRepository(NemeStatsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public virtual List<GameDefinition> GetAllGameDefinitions(UserContext userContext)
        {
            return dbContext.GameDefinitions
                .Where(game => game.GamingGroupId == userContext.GamingGroupId)
                .ToList();
        }
        
        public virtual GameDefinition GetGameDefinition(
            int gameDefinitionId, 
            UserContext userContext)
        {
            GameDefinition game = dbContext.GameDefinitions.Where(gameDefinition => gameDefinition.Id == gameDefinitionId).FirstOrDefault();
            ValidateGameDefinitionIsFound(gameDefinitionId, game);

            ValidateUserHasAccessToGameDefinition(userContext, game);

            return game;
        }

        private static void ValidateGameDefinitionIsFound(int gameDefinitionId, GameDefinition game)
        {
            if (game == null)
            {
                string keyNotFoundMessage = string.Format(EXCEPTION_MESSAGE_GAME_DEFINITION_NOT_FOUND, gameDefinitionId);
                throw new KeyNotFoundException(keyNotFoundMessage);
            }
        }

        internal virtual void ValidateUserHasAccessToGameDefinition(UserContext userContext, GameDefinition game)
        {
            if (game.GamingGroupId != userContext.GamingGroupId)
            {
                string notAuthorizedMessage = string.Format(
                    EXCEPTION_MESSAGE_USER_DOES_NOT_HAVE_ACCESS_TO_GAME_DEFINITION,
                    userContext.ApplicationUserId,
                    game.Id);
                throw new UnauthorizedAccessException(notAuthorizedMessage);
            }
        }

        public virtual GameDefinition Save(GameDefinition gameDefinition, UserContext userContext)
        {
            if(gameDefinition.AlreadyInDatabase())
            {
                ValidateUserHasAccessToGameDefinition(userContext, gameDefinition);
                dbContext.Entry(gameDefinition).State = System.Data.Entity.EntityState.Modified;
            }else
            {
                //TODO should throw some kind of exception if GamingGroupId is null
                gameDefinition.GamingGroupId = userContext.GamingGroupId.Value;
                dbContext.GameDefinitions.Add(gameDefinition);
            }
            
            dbContext.SaveChanges();

            return gameDefinition;
        }

        public virtual void Delete(int gameDefinitionId, UserContext userContext)
        {
            GameDefinition gameDefinition = GetGameDefinition(gameDefinitionId, userContext);

            dbContext.GameDefinitions.Remove(gameDefinition);
            dbContext.SaveChanges();
        }
    }
}
