using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BusinessLogic.Models.Utility
{
    public class BasicDateRangeFilter : IDateRangeFilter
    {
        public readonly DateTime DefaultFromDate = new DateTime(2010, 1, 1);
        public readonly DateTime DefaultToDate;

        public BasicDateRangeFilter()
        {
            _fromDate = DefaultFromDate.Date;
            DefaultToDate = DateTime.UtcNow.Date.AddDays(1).AddMilliseconds(-1);
            _toDate = DefaultToDate;
        }

        public bool HasCustomDate
        {
            get { return FromDate != DefaultFromDate || ToDate != DefaultToDate; }
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

        [RegularExpression(@"^(19|20)\d\d[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])$", ErrorMessage = "Date must be in the format YYYY-MM-DD")]
        public string Iso8601FromDate
        {
            get
            {
                return FromDate.ToString("yyyy-MM-dd");
            }
            set
            {
                _fromDate = DateTime.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
        }

        [RegularExpression(@"^(19|20)\d\d[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])$", ErrorMessage = "Date must be in the format YYYY-MM-DD")]
        public string Iso8601ToDate
        {
            get
            {
                return ToDate.ToString("yyyy-MM-dd");
            }
            set
            {
                _toDate = DateTime.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
        }
    }
}
