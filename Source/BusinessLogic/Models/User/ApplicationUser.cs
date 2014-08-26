using BusinessLogic.DataAccess;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models.User
{
    public class ApplicationUser : IdentityUser, SingleColumnTechnicalKey<string>, EntityWithTechnicalKey
    {
        public virtual int? CurrentGamingGroupId { get; set; }
        /// <summary>
        /// This is pulled from the analytics token and can not be linked back to the actual user. Used for pushing analytics
        /// events and should not be persisted to the DB.
        /// </summary>
        [NotMapped]
        public virtual string AnonymousClientId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual IList<UserGamingGroup> UserGamingGroups { get; set; }

        public bool AlreadyInDatabase()
        {
            return Id != null;
        }
    }
}
