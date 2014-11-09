using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;

namespace BusinessLogic.Models.GamingGroups
{
    public class GamingGroupSummary
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string OwningUserId { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual ApplicationUser OwningUser { get; set; }
        public virtual IList<Player> Players { get; set; }
        public virtual IList<GameDefinitionSummary> GameDefinitionSummaries { get; set; }
        public virtual IList<PlayedGame> PlayedGames { get; set; }
        public virtual IList<GamingGroupInvitation> GamingGroupInvitations { get; set; }
    }
}
