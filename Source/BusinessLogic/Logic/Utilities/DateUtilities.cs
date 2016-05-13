using System;

namespace BusinessLogic.Logic.Utilities
{
    public class DateUtilities : IDateUtilities
    {
        public int GetNumberOfSecondsUntilEndOfDay()
        {
            var timeSpanUntilUtcMidnight = DateTime.UtcNow.Date.AddDays(1)- DateTime.UtcNow;
            return (int)timeSpanUntilUtcMidnight.TotalSeconds;
        }
    }
}