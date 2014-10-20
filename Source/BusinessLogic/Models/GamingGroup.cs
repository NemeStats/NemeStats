using BusinessLogic.DataAccess;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class GamingGroup : EntityWithTechnicalKey<int>
    {
        public GamingGroup()
        {
            DateCreated = DateTime.UtcNow;
        }

        public override int Id { get; set; }
        public string Name { get; set; }

        public string OwningUserId { get; set; }
        public DateTime DateCreated { get; set; }

        [ForeignKey("OwningUserId")]
        public virtual ApplicationUser OwningUser { get; set; }
        public virtual IList<Player> Players { get; set; }
        public virtual IList<GameDefinition> GameDefinitions { get; set; }
        public virtual IList<PlayedGame> PlayedGames { get; set; }
        public virtual IList<GamingGroupInvitation> GamingGroupInvitations { get; set; }
    }
}
