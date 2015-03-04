#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using BusinessLogic.Exceptions;
using BusinessLogic.Models.User;
using System;
using System.Linq;

namespace BusinessLogic.DataAccess.Security
{
    public class SecuredEntityValidator<TEntity> : ISecuredEntityValidator<TEntity> where TEntity : class
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
