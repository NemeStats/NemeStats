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

        public virtual List<GameDefinition> GetAllGameDefinitions(ApplicationUser currentUser)
        {
            return dbContext.GameDefinitions
                .Where(game => game.GamingGroupId == currentUser.CurrentGamingGroupId)
                .ToList();
        }
        
        public virtual GameDefinition GetGameDefinition(
            int gameDefinitionId, 
            ApplicationUser currentUser)
        {
            GameDefinition game = dbContext.GameDefinitions.Where(gameDefinition => gameDefinition.Id == gameDefinitionId).FirstOrDefault();
            ValidateGameDefinitionIsFound(gameDefinitionId, game);

            ValidateUserHasAccessToGameDefinition(currentUser, game);

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

        public virtual GameDefinition Save(GameDefinition gameDefinition, ApplicationUser currentUser)
        {
            if(gameDefinition.AlreadyInDatabase())
            {
                ValidateUserHasAccessToGameDefinition(currentUser, gameDefinition);
                dbContext.Entry(gameDefinition).State = System.Data.Entity.EntityState.Modified;
            }else
            {
                //TODO should throw some kind of exception if GamingGroupId is null
                gameDefinition.GamingGroupId = currentUser.CurrentGamingGroupId.Value;
                dbContext.GameDefinitions.Add(gameDefinition);
            }
            
            dbContext.SaveChanges();

            return gameDefinition;
        }

        public virtual void Delete(int gameDefinitionId, ApplicationUser currentUser)
        {
            GameDefinition gameDefinition = GetGameDefinition(gameDefinitionId, currentUser);

            dbContext.GameDefinitions.Remove(gameDefinition);
            dbContext.SaveChanges();
        }
    }
}
