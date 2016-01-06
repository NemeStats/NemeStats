using System;

namespace BusinessLogic.Models.Utility
{
    public class BasicDateRangeFilter : IDateRangeFilter
    {
        public readonly DateTime DefaultFromDate = new DateTime(2010, 1, 1);

        public BasicDateRangeFilter()
        {
            _fromDate = DefaultFromDate.Date;
            _toDate = DateTime.UtcNow.Date.AddDays(1).AddMilliseconds(-1);
        }

        private DateTime _fromDate;

        public DateTime FromDate
        {
            get
            {
                return _fromDate;
            }
            set
            {
                _fromDate = value.Date;
            }
        }
        private DateTime _toDate;

        public DateTime ToDate
        {
            get
            {
                return _toDate;
            }
            set
            {
                //set the date to the very end of the day
                _toDate = value.Date.AddDays(1).AddMilliseconds(-1);
            }
        }
    }
}
