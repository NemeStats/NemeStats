using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Games.Validation;

namespace UI.Areas.Api.Models
{
    public abstract class SaveablePlayedGameMessageBase
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

        public IList<ApplicationLinkageMessage> ApplicationLinkages { get; set; }
    }
}