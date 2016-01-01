using BusinessLogic.Models.Utility;
using System;

namespace UI.Models.GamingGroup
{
    public class GamingGroupRequest : IDateRangeFilter
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int GamingGroupId { get; set; }
    }
}