using System.Linq;
using BusinessLogic.Models;

namespace BusinessLogic.Logic.GamingGroups
{
    public class GamingGroupInfoForUser
    {
        public int GamingGroupId { get; set; }
        public string GamingGroupName { get; set; }
        public string GamingGroupPublicUrl { get; set; }
        public string GamingGroupPublicDescription { get; set; }
        public int NumberOfGamesPlayed { get; set; }
        public int NumberOfPlayers { get; set; }
        public bool Active { get; set; }
        public Player GamingGroupChampion { get; set; }
    }
}
