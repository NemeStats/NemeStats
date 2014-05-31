using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class NewlyCompletedGame
    {
        public int GameDefinitionId { get; set; }

        public virtual List<PlayerRank> PlayerRanks { get; set; }
    }
}
