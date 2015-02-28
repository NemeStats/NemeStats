using System;

namespace BusinessLogic.Models.PlayedGames
{
    public class PlayedGameExportModel
    {
        public int Id { get; set; }
        public int GamingGroupId { get; set; }
        public string GamingGroupName { get; set; }
        public int GameDefinitionId { get; set; }
        public int? BoardGameGeekObjectId { get; set; }
        public string GameDefinitionName { get; set; }
        public DateTime DatePlayed { get; set; }
        public DateTime DateCreated { get; set; }
        public int NumberOfPlayers { get; set; }
        public string WinningPlayerIds { get; set; }
        public string WinningPlayerNames { get; set; }
        public string Notes { get; set; }
    }
}
