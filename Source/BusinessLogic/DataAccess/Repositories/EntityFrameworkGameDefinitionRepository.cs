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
            SecuredEntityValidatorFactory securedEntityValidatorFactory)
        {
            this.dataContext = dataContext;
            this.securedEntityValidatorFactory = securedEntityValidatorFactory;
        }

        public virtual List<GameDefinition> GetAllGameDefinitions(ApplicationUser currentUser)
        {
            return dataContext.GetQueryable<GameDefinition>(currentUser)
                .Where(game => game.GamingGroupId == currentUser.CurrentGamingGroupId)
                .ToList();
        }
    }
}
