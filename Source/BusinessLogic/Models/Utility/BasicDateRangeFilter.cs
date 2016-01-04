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
            _fromDate = DefaultFromDate.Date;
            _toDate = DateTime.UtcNow.Date;
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
                _toDate = value.Date;
            }
        }
    }
}
