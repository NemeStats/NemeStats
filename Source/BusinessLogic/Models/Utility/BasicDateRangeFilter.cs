using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BusinessLogic.Models.Utility
{
    public class BasicDateRangeFilter : IDateRangeFilter
    {
        public readonly DateTime DefaultFromDate = new DateTime(2014, 1, 1);
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

        public string Iso8601FromDate
        {
            get
            {
                return FromDate.ToString("yyyy-MM-dd");
            }
            set
            {
                _fromDate = ConvertIso8601DateToDateTime(value);
            }
        }

        public string Iso8601ToDate
        {
            get
            {
                return ToDate.ToString("yyyy-MM-dd");
            }
            set
            {
                _toDate = ConvertIso8601DateToDateTime(value);
            }
        }

        private DateTime ConvertIso8601DateToDateTime(string value)
        {
            DateTime parsedDate;
            if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                return parsedDate;
            }
            else
            {
                throw new FormatException(string.Format("'{0}' is not a valid YYYY-MM-DD date.", value));
            }
        }
    }
}
