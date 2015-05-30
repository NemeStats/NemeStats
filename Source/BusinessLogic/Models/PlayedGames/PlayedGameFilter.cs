using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Models.PlayedGames
{
    public class PlayedGameFilter
    {
        public string StartDateGameLastUpdated { get; set; }
        public string EndDateGameLastUpdated { get; set; }
        public int? MaximumNumberOfResults { get; set; }
        public int? GamingGroupId { get; set; }
        public int? GameDefinitionId { get; set; }
    }
}
