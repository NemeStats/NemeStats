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
using System;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Users
{
    public class GamingGroupContextSwitcher : IGamingGroupContextSwitcher
    {
        public const string EXCEPTION_MESSAGE_NO_ACCESS = "User with Id '{0}' does not have access to GamingGroup with Id '{1}'";

        private readonly IDataContext _dataContext;

        public GamingGroupContextSwitcher(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void SwitchGamingGroupContext(int gamingGroupId, ApplicationUser currentUser)
        {
            if (gamingGroupId == currentUser.CurrentGamingGroupId)
            {
                return;
            }

            var hasAccess  = _dataContext.GetQueryable<UserGamingGroup>()
                                         .Any(userGamingGroup => userGamingGroup.ApplicationUserId == currentUser.Id
                                                                 && userGamingGroup.GamingGroupId == gamingGroupId);

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException(string.Format(EXCEPTION_MESSAGE_NO_ACCESS, currentUser.Id, gamingGroupId));
            }

            var user = _dataContext.FindById<ApplicationUser>(currentUser.Id);
            user.CurrentGamingGroupId = gamingGroupId;
            _dataContext.Save(user, currentUser);
        }

        public void EnsureContextIsValid(ApplicationUser user)
        {
            if (user.CurrentGamingGroupId.HasValue)
            {
                var currentGamingGroupIsValid = _dataContext
                    .GetQueryable<UserGamingGroup>()
                    .Any(x => x.GamingGroupId == user.CurrentGamingGroupId.Value
                        && x.ApplicationUserId == user.Id
                        && x.GamingGroup.Active);
                if (currentGamingGroupIsValid)
                {
                    return;
                }
            }

            var activeGamingGroup = _dataContext
                .GetQueryable<UserGamingGroup>()
                .OrderBy(x => x.GamingGroup.DateCreated)
                .FirstOrDefault(x => x.ApplicationUserId == user.Id && x.GamingGroup.Active);

            if (activeGamingGroup == null)
            {
                user.CurrentGamingGroupId = null;
            }
            else
            {
                user.CurrentGamingGroupId = activeGamingGroup.GamingGroupId;
            }
            _dataContext.AdminSave(user);

        }
    }
}
