using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Helpers
{
    public class TimeConverterHelper
    {
        public static DateTime GetDatetimeFromUnixTime(long unixTime)
        {
            if(unixTime < DateTimeOffset.MinValue.ToUnixTimeSeconds() || unixTime> DateTimeOffset.MaxValue.ToUnixTimeSeconds())
            {
                throw new ArgumentOutOfRangeException(nameof(unixTime), "Unix time is out of range.");
            }

            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTime);
            return dateTimeOffset.DateTime;
        }
    }
}
