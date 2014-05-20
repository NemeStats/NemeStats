using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class PlayedGame
    {
        public int Id { get; set; }

        public int GameDefinitionId { get; set; }
        public int NumberOfPlayers { get; set; }

        public virtual GameDefinition GameDefinition { get; set; }
        public virtual ICollection<PlayerGameResult> PlayerGameResults { get; set; }
    }
}
