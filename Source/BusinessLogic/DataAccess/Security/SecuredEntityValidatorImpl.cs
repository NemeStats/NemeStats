using BusinessLogic.Exceptions;
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
        internal const string EXCEPTION_MESSAGE_CURRENT_USER_ID_CANNOT_BE_NULL
           = "currentUser.Id cannot be null";
        internal const string EXCEPTION_MESSAGE_CURRENT_USER_GAMING_GROUP_ID_CANNOT_BE_NULL
            = "currentUser.CurrentGamingGroupId cannot be null";

        //TODO not sure how to enforce that TEntity is a SingleColumnWithTechnicalKey so I can get the Id, so requiring some
        //additional info to be manually passed in
        public virtual void ValidateAccess(TEntity entity, ApplicationUser currentUser, Type underlyingEntityType, object entityId)
        {
            SecuredEntityWithTechnicalKey securedEntity = entity as SecuredEntityWithTechnicalKey;

            if (securedEntity == null)
            {
                return;
            }
            
            ValidateArguments(currentUser);

            if(securedEntity.GamingGroupId != currentUser.CurrentGamingGroupId)
            {
                throw new UnauthorizedEntityAccessException(currentUser.Id, underlyingEntityType, entityId);
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
