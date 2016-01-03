using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models.Utility
{
    public class BasicDateRangeFilter : IDateRangeFilter
    {
        public readonly DateTime DefaultFromDate = new DateTime(2010, 1, 1);

        public BasicDateRangeFilter()
        {
            FromDate = DefaultFromDate;
            ToDate = DateTime.UtcNow;
        }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
