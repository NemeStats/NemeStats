using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Games.Validation;

namespace UI.Areas.Api.Models
{
    public class PlayedGameMessage : ISynchable
    {
        [RegularExpression(@"^\d\d\d\d-\d\d\-\d\d$", ErrorMessage = "Dates must be in the format of YYYY-MM-DD.")]
        public string DatePlayed { get; set; }

        [Required]
        public int? GameDefinitionId { get; set; }

        public string Notes { get; set; }

        [Required]
        [PlayerRankValidation]
        public List<PlayerRank> PlayerRanks { get; set; }

        public int? GamingGroupId { get; set; }

        public string ExternalSourceApplicationName { get; set; }
        public string ExternalSourceEntityId { get; set; }
    }
}
