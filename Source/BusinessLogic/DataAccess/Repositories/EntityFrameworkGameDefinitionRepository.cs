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

        private NemeStatsDbContext dbContext;
        internal const string EXCEPTION_MESSAGE_GAME_DEFINITION_NOT_FOUND
            = "Game Definition with Id '{0}' not found.";

        public EntityFrameworkGameDefinitionRepository(NemeStatsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public virtual List<GameDefinition> GetAllGameDefinitions(NemeStatsDbContext dbContext, UserContext userContext)
        {
            return dbContext.GameDefinitions
                .Where(game => game.GamingGroupId == userContext.GamingGroupId)
                .ToList();
        }
        
        public virtual GameDefinition GetGameDefinition(
            int gameDefinitionId, 
            NemeStatsDbContext dbContext, 
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
                gameDefinition.GamingGroupId = userContext.GamingGroupId;
                dbContext.GameDefinitions.Add(gameDefinition);
            }
            
            dbContext.SaveChanges();

            return gameDefinition;
        }

        public virtual void Delete(int gameDefinitionId, NemeStatsDbContext dbContext, UserContext userContext)
        {
            GameDefinition gameDefinition = GetGameDefinition(gameDefinitionId, dbContext, userContext);

            dbContext.GameDefinitions.Remove(gameDefinition);
            dbContext.SaveChanges();
        }
    }
}
