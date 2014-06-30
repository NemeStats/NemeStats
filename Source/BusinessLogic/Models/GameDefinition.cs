using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class GameDefinition
    {
        public int Id { get; set; }

        public int GamingGroupId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public virtual IList<PlayedGame> PlayedGames { get; set; }
        public virtual GamingGroup GamingGroup { get; set; }
    }
}
