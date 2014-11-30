using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Models.Players
{
    public class PlayerDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ApplicationUserId { get; set; }
        public bool Active { get; set; }
        public int GamingGroupId { get; set; }
        public string GamingGroupName { get; set; }

        public PlayerStatistics PlayerStats { get; set; }
        public List<PlayerGameResult> PlayerGameResults { get; set; }
        public Nemesis CurrentNemesis { get; set; }
        public Nemesis PreviousNemesis { get; set; }
        public List<Player> Minions { get; set; }
        public List<PlayerGameSummary> PlayerGameSummaries { get; set; }
        public List<Champion> Championships { get; set; } 
    }
}
