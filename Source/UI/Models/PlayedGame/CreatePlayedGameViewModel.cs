using System.Collections.Generic;
using BusinessLogic.Models.Players;
using UI.Models.GameDefinitionModels;

namespace UI.Models.PlayedGame
{
    public class CreatePlayedGameViewModel
    {
        public IList<GameDefinitionDisplayInfoViewModel> RecentPlayedGames { get; set; }
        public IList<GameDefinitionDisplayInfoViewModel> MostPlayedGames { get; set; }
        public List<PlayerInfoForUser> RecentPlayers { get; set; }
        public List<PlayerInfoForUser> MostActivePlayers { get; set; }
        public List<PlayerInfoForUser> OtherPlayers { get; set; }
        public PlayerInfoForUser UserPlayer { get; set; }

    }
}