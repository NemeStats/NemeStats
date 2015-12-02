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
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BusinessLogic.Models.User
{
    public class ApplicationUser : IdentityUser, ISingleColumnTechnicalKey<string>, EntityWithTechnicalKey
    {
        public ApplicationUser()
        {
            DateCreated = DateTime.UtcNow;
        }

        public virtual int CurrentGamingGroupId { get; set; }
        /// <summary>
        /// This is pulled from the analytics token and can not be linked back to the actual user. Used for pushing analytics
        /// events and should not be persisted to the DB.
        /// </summary>
        [NotMapped]
        public virtual string AnonymousClientId { get; set; }
        public DateTime DateCreated { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual IList<UserGamingGroup> UserGamingGroups { get; set; }
        [ForeignKey("ApplicationUserId")]
        public virtual IList<Player> Players { get; set; }

        public string AuthenticationToken { get; set; }

        public bool AlreadyInDatabase()
        {
            return Id != null;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public DateTime? AuthenticationTokenExpirationDate { get; set; }
    }
}
