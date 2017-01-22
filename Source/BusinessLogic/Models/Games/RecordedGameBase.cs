using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BusinessLogic.Models.Games.Validation;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.Validation;

namespace BusinessLogic.Models.Games
{
    public abstract class SaveableGameBase
    {
        protected SaveableGameBase()
        {
            DatePlayed = DateTime.UtcNow;
            ApplicationLinkages = new List<ApplicationLinkage>();
            PlayerRanks = new List<PlayerRank>();
        }

        public int GameDefinitionId { get; set; }

        public string Notes { get; set; }

        [PlayerRankValidation]
        [Required]
        public List<PlayerRank> PlayerRanks { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [MaxDate]
        public DateTime DatePlayed { get; set; }

        public int? GamingGroupId { get; set; }

        public IList<ApplicationLinkage> ApplicationLinkages { get; set; }
    }
}
