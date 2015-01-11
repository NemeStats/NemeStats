using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Models.GamingGroups
{
    public class TopGamingGroupSummary
    {
        public string GamingGroupName { get; set; }
        public int GamingGroupId { get; set; }
        public int NumberOfPlayers { get; set; }
        public int NumberOfGamesPlayed { get; set; }
    }
}
