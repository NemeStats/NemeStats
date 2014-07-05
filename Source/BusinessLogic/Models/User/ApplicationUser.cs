using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models.User
{
    public class ApplicationUser : IdentityUser
    {
        public int CurrentGamingGroupId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual IList<UserGamingGroup> UserGamingGroups { get; set; }
    }
}
