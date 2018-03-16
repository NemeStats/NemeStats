using BusinessLogic.Models.Utility;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Models.GamingGroups
{
    public class GamingGroupFilter
    {
        public GamingGroupFilter(int gamingGroupId, IDateRangeFilter dateRangeFilter = null)
        {
            GamingGroupId = gamingGroupId;
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
        public int GamingGroupId { get; private set; }
    }
}
