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

        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<PlayedGame> PlayedGames { get; set; }
        public ICollection<PlayerGameResult> PlayerGameResults { get; set;  }
    }
}
