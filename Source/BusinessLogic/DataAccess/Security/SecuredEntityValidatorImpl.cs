using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess.Security
{
    public class SecuredEntityValidatorImpl<TEntity> : SecuredEntityValidator<TEntity> where TEntity : class
    {
        internal const string EXCEPTION_MESSAGE_USER_DOES_NOT_HAVE_ACCESS_TO_GAME_DEFINITION
           = "User with Id '{0}' is unauthorized to access the given entity of type '{1}'";
        internal const string EXCEPTION_MESSAGE_CURRENT_USER_ID_CANNOT_BE_NULL
           = "currentUser.Id cannot be null";
        internal const string EXCEPTION_MESSAGE_CURRENT_USER_GAMING_GROUP_ID_CANNOT_BE_NULL
            = "currentUser.CurrentGamingGroupId cannot be null";

        public virtual void ValidateAccess(TEntity entity, ApplicationUser currentUser, Type underlyingEntityType)
        {
            SecuredEntityWithTechnicalKey securedEntity = entity as SecuredEntityWithTechnicalKey;

            if (securedEntity == null)
            {
                return;
            }

            ValidateArguments(currentUser);

            if(securedEntity.GamingGroupId != currentUser.CurrentGamingGroupId)
            {
                string message = string.Format(EXCEPTION_MESSAGE_USER_DOES_NOT_HAVE_ACCESS_TO_GAME_DEFINITION,
                    currentUser.Id,
                    underlyingEntityType.ToString());
                throw new UnauthorizedAccessException(message);
            }
        }

        private static void ValidateArguments(ApplicationUser currentUser)
        {
            if (currentUser == null)
            {
                throw new ArgumentNullException("currentUser");
            }

            if (currentUser.Id == null)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_CURRENT_USER_ID_CANNOT_BE_NULL);
            }

            if (currentUser.CurrentGamingGroupId == null)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_CURRENT_USER_GAMING_GROUP_ID_CANNOT_BE_NULL);
            }
        }
    }
}
