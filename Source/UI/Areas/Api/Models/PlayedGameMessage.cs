using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using BusinessLogic.Models.Games;

namespace UI.Areas.Api.Models
{
    public class PlayedGameMessage
    {
        [RegularExpression(@"^\d\d\d\d-\d\d\-\d\d$", ErrorMessage = "Dates must be in the format of YYYY-MM-DD.")]
        public string DatePlayed { get; set; }

        [Required]
        public int? GameDefinitionId { get; set; }

        public string Notes { get; set; }

        [Required]
        public List<PlayerRank> PlayerRanks { get; set; }
    }
}
