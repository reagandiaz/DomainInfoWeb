using System;

namespace IntegrationTools.DataConversion
{
    public class DateTimeHelper
    {
        public DateTime ConvertUTCToDateTime(long mt)
        {
            return (new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).AddSeconds(mt / 1000);
        }

        public long ConvertUTCDateTimeToLong(DateTime dt)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dt - sTime).TotalSeconds * 1000;
        }
    }
}
