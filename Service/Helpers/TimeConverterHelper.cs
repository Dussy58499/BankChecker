using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Helpers
{
    public class TimeConverterHelper
    {
        public static DateTime GetDatetimeFromUnixTime(long unixTime)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTime);
            return dateTimeOffset.DateTime;
        }
    }
}
