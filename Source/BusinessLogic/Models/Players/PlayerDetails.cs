using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models.Players
{
    public class PlayerDetails
    {
        //TODO so, so smelly. Tried extending Player but that caused weird entity framework code first stuff to happen.  Perhaps share
        //a common interface instead?
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }

        public PlayerStatistics PlayerStats { get; set; }

        public List<PlayerGameResult> PlayerGameResults { get; set; }
    }
}
