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

        private IDataContext _dataContext;

        public SecuredEntityValidator(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        //TODO not sure how to enforce that TEntity is a SingleColumnWithTechnicalKey so I can get the Id, so requiring some
        //additional info to be manually passed in
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="currentUser"></param>
        /// <param name="entityId"></param>
        /// <exception cref="UnauthorizedAccessException">Thrown if the user is not allowed to save this entity because the user does not have access to the gaming group.</exception>
        public virtual void ValidateAccess(TEntity entity, ApplicationUser currentUser, object entityId)
        {
            var securedEntity = entity as SecuredEntityWithTechnicalKey;

            if (securedEntity == null)
            {
                return;
            }
            
            ValidateArguments(currentUser);

            if(securedEntity.GamingGroupId != default(int) && securedEntity.GamingGroupId != currentUser.CurrentGamingGroupId)
            {
                var matchingUserGamingGroup = _dataContext.GetQueryable<UserGamingGroup>()
                    .SingleOrDefault(
                                    x =>
                                    x.GamingGroupId == securedEntity.GamingGroupId &&
                                    x.ApplicationUserId == currentUser.Id);

                if (matchingUserGamingGroup == null)
                {
                    throw new UnauthorizedEntityAccessException(currentUser.Id, typeof(TEntity), entityId);
                }
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
        }
    }
}
