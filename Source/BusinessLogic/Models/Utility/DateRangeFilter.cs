using System;

namespace BusinessLogic.Models.Utility
{
    public interface IDateRangeFilter
    {
        DateTime? FromDate { get; set; }
        DateTime? ToDate { get; set; }
    }
}
