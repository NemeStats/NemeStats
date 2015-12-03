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
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Linq;

namespace BusinessLogic.DataAccess.GamingGroups
{
    public class EntityFrameworkGamingGroupAccessGranter : IGamingGroupAccessGranter
    {
        protected IDataContext dataContext;

        public EntityFrameworkGamingGroupAccessGranter(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public GamingGroupInvitation CreateInvitation(string email, ApplicationUser currentUser)
        {
            GamingGroupInvitation invitation = new GamingGroupInvitation()
            {
                InviteeEmail = email,
                GamingGroupId = currentUser.CurrentGamingGroupId,
                InvitingUserId = currentUser.Id,
                DateSent = DateTime.UtcNow.Date
            };
            dataContext.Save<GamingGroupInvitation>(invitation, currentUser);

            return invitation;
        }


        public GamingGroupInvitation ConsumeInvitation(GamingGroupInvitation gamingGroupInvitation, ApplicationUser currentUser)
        {
            gamingGroupInvitation.DateRegistered = DateTime.UtcNow;
            gamingGroupInvitation.RegisteredUserId = currentUser.Id;
            return dataContext.Save<GamingGroupInvitation>(gamingGroupInvitation, currentUser);
        }
    }
}
