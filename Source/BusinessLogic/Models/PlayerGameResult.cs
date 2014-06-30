using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class PlayerGameResult
    {
        public int Id { get; set; }

        public int GamingGroupId { get; set; }
        public int PlayedGameId { get; set; }
        public int PlayerId { get; set; }
        public int GameRank { get; set; }
        public int GordonPoints { get; set; }

        public virtual GamingGroup GamingGroup { get; set; }
        public virtual PlayedGame PlayedGame { get; set; }
        public virtual Player Player { get; set; }
    }
}
