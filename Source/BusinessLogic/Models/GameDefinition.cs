using BusinessLogic.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Models
{
    public class GameDefinition : SecuredEntityWithTechnicalKey<int>
    {
        public GameDefinition()
        {
            this.Active = true;
        }

        public override int Id { get; set; }

        public override int GamingGroupId { get; set; }

        public int? BoardGameGeekObjectId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        public virtual IList<PlayedGame> PlayedGames { get; set; }
        public virtual GamingGroup GamingGroup { get; set; }
    }
}
