using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [ForeignKey("ApplicationUserId")]
        public virtual IList<UserGamingGroup> UserGamingGroups { get; set; }
    }
}
