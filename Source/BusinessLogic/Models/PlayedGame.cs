using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class PlayedGame
    {
        public int Id { get; set; }

        public int GamingGroupId { get; set; }
        public int GameDefinitionId { get; set; }
        public int NumberOfPlayers { get; set; }
        public DateTime DatePlayed { get; set; }

        public virtual GamingGroup GamingGroup { get; set; }
        public virtual GameDefinition GameDefinition { get; set; }
        public virtual IList<PlayerGameResult> PlayerGameResults { get; set; }
    }
}
