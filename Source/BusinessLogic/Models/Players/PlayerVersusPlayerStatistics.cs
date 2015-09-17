using System.Linq;

namespace BusinessLogic.Models.Players
{
    public class PlayerVersusPlayerStatistics
    {
        public string OpposingPlayerName { get; set; }
        public int OpposingPlayerId { get; set; }
        public int NumberOfGamesWonVersusThisPlayer { get; set; }
        public int NumberOfGamesLostVersusThisPlayer { get; set; }
    }
}
