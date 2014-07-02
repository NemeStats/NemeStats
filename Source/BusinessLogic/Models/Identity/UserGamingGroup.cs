using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models.Identity
{
    public class UserGamingGroup
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        public int GamingGroupId { get; set; }

        public virtual GamingGroup GamingGroup { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
