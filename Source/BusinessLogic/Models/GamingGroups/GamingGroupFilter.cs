using BusinessLogic.Models.Utility;
using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Models.GamingGroups
{
    public class GamingGroupFilter : IDateRangeFilter
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        [Required]
        public int GamingGroupId { get; set; }
        public int NumberOfRecentGamesToShow { get; set; }
    }
}
