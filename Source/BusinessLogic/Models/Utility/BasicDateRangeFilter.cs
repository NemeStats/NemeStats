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

        public virtual bool HasCustomDate
        {
            get { return FromDate != DefaultFromDate || ToDate != DefaultToDate; }
        }

        private DateTime _fromDate;

        public virtual DateTime FromDate
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

        public virtual DateTime ToDate
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

        public virtual string Iso8601FromDate
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

        public virtual string Iso8601ToDate
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
            throw new FormatException($"'{value}' is not a valid YYYY-MM-DD date.");
        }

        public virtual bool IsValid(out string errorMessage)
        {
            errorMessage = null;
            DateTime twoDaysInTheFuture = DateTime.UtcNow.Date.AddDays(1).AddMilliseconds(-1);
            if (ToDate > twoDaysInTheFuture)
            {
                errorMessage = "The 'To Date' cannot be in the future.";
                return false;
            }

            if (FromDate > twoDaysInTheFuture)
            {
                errorMessage = "The 'From Date' cannot be in the future.";
                return false;
            }

            if (ToDate < FromDate)
            {
                errorMessage = "The 'From Date' cannot be greater than the 'To Date'.";
                return false;
            }

            return true;
        }
    }
}
