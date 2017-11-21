using System;
using System.Collections.Generic;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Players;

namespace BusinessLogic.Models.PlayedGames
{
    public class EditPlayedGameInfo
    {
        public PlayerInfoForUser UserPlayer { get; set; }
        public List<PlayerInfoForUser> OtherPlayers { get; set; }
        public List<PlayerInfoForUser> RecentPlayers { get; set; }
        public DateTime DatePlayed { get; set; }
        public string Notes { get; set; }
        public int GameDefinitionId { get; set; }
        public string GameDefinitionName { get; set; }
        public int? BoardGameGeekGameDefinitionId { get; set; }
        public List<PlayerRankWithName> PlayerRanks { get; set; }
    }
}
