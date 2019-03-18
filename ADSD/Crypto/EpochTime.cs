using System;

namespace ADSD.Crypto
{
    /// <summary>
    /// Returns the absolute DateTime or the Seconds since Unix Epoch, where Epoch is UTC 1970-01-01T0:0:0Z.
    /// </summary>
    public static class EpochTime
    {
        /// <summary>DateTime as UTV for UnixEpoch</summary>
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Per JWT spec:
        /// Gets the number of seconds from 1970-01-01T0:0:0Z as measured in UTC until the desired date/time.
        /// </summary>
        /// <param name="datetime">The DateTime to convert to seconds.</param>
        /// <remarks>if dateTimeUtc less than UnixEpoch, return 0</remarks>
        /// <returns>the number of seconds since Unix Epoch.</returns>
        public static long GetIntDate(DateTime datetime)
        {
            DateTime dateTime = datetime;
            if (datetime.Kind != DateTimeKind.Utc)
                dateTime = datetime.ToUniversalTime();
            if (dateTime.ToUniversalTime() <= EpochTime.UnixEpoch)
                return 0;
            return (long) (dateTime - EpochTime.UnixEpoch).TotalSeconds;
        }

        /// <summary>Creates a DateTime from epoch time.</summary>
        /// <param name="secondsSinceUnixEpoch">Number of seconds.</param>
        /// <returns>The DateTime in UTC.</returns>
        public static DateTime DateTime(long secondsSinceUnixEpoch)
        {
            if (secondsSinceUnixEpoch <= 0L)
                return EpochTime.UnixEpoch;
            return DateTimeUtil.Add(EpochTime.UnixEpoch, TimeSpan.FromSeconds((double) secondsSinceUnixEpoch)).ToUniversalTime();
        }
    }
}
