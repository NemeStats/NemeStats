namespace BusinessLogic.Models.GamingGroups
{
    public class GamingGroupStats
    {
        public static NullGamingGroupStats NullStats = new NullGamingGroupStats();
        public int TotalPlayedGames { get; set; }
        public int DistinctGamesPlayed { get; set; }

        public class NullGamingGroupStats : GamingGroupStats
        {

        }
    }


}