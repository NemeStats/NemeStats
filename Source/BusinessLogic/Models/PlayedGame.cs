using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class PlayedGame
    {
        public int GameDefinitionID { get; set; }
        public int NumberOfPlayers { get; set; }

        public virtual ICollection<Player> Players { get; set; }
    }
}
