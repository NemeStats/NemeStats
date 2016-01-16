using BusinessLogic.Models.Utility;
using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Models.GamingGroups
{
    public class GamingGroupFilter
    {
        public GamingGroupFilter(IDateRangeFilter dateRangeFilter = null)
        {
            if(dateRangeFilter == null)
            {
                DateRangeFilter = new BasicDateRangeFilter();
            }else
            {
                DateRangeFilter = dateRangeFilter;
            }
        }

        public IDateRangeFilter DateRangeFilter { get; private set; }
        [Required]
        public int GamingGroupId { get; set; }
        public int NumberOfRecentGamesToShow { get; set; }
    }
}
