using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models.Players
{
    public class PlayerDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public int GamingGroupId { get; set; }

        public PlayerStatistics PlayerStats { get; set; }
        public List<PlayerGameResult> PlayerGameResults { get; set; }
        public Nemesis PlayerNemesis { get; set; }
        public List<Player> Minions { get; set; }
    }
}
