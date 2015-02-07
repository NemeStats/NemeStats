using System;
using System.Linq;
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Models.Games
{
    public class PublicGameSummary
    {
        public int PlayedGameId { get; set; }
        public DateTime DatePlayed { get; set; }
        public int GameDefinitionId { get; set; }
        public string GameDefinitionName { get; set; }
        public string GamingGroupName { get; set; }
        public int GamingGroupId { get; set; }
        public IWinner Winner { get; set; }
        public WinnerTypes WinnerType { get; set; }
        public Player WinningPlayer { get; set; }
    }
}
