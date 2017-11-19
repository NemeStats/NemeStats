using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Games.Validation;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.Validation;
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
        public bool EditMode { get; set; }
        public int? PlayedGameId { get; set; }
        public PlayerInfoForUser UserPlayer { get; set; }
    }

    public class EditPlayedGameViewModel : CreatePlayedGameViewModel
    {
        public int GameDefinitionId { get; set; }
        public int? BoardGameGeekGameDefinitionId { get; set; }
        public string GameDefinitionName { get; set; }

        public string Notes { get; set; }

        [PlayerRankValidation]
        [Required]
        public List<PlayerRankWithName> PlayerRanks { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [MaxDate]
        public DateTime DatePlayed { get; set; }
    }
}