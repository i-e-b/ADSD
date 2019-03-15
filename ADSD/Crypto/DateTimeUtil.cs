using System;

namespace ADSD
{
    /// <summary>Helper class for adding DateTimes and Timespans.</summary>
    internal static class DateTimeUtil
    {
        /// <summary>
        /// Add a DateTime and a TimeSpan.
        /// The maximum time is DateTime.MaxTime.  It is not an error if time + timespan &gt; MaxTime.
        /// Just return MaxTime.
        /// </summary>
        /// <param name="time">Initial <see cref="T:System.DateTime" /> value.</param>
        /// <param name="timespan"><see cref="T:System.TimeSpan" /> to add.</param>
        /// <returns><see cref="T:System.DateTime" /> as the sum of time and timespan.</returns>
        public static DateTime Add(DateTime time, TimeSpan timespan)
        {
            if (timespan == TimeSpan.Zero)
                return time;
            if (timespan > TimeSpan.Zero && DateTime.MaxValue - time <= timespan)
                return DateTimeUtil.GetMaxValue(time.Kind);
            if (timespan < TimeSpan.Zero && DateTime.MinValue - time >= timespan)
                return DateTimeUtil.GetMinValue(time.Kind);
            return time + timespan;
        }

        /// <summary>
        /// Gets the Maximum value for a DateTime specifying kind.
        /// </summary>
        /// <param name="kind">DateTimeKind to use.</param>
        /// <returns>DateTime of specified kind.</returns>
        public static DateTime GetMaxValue(DateTimeKind kind)
        {
            if (kind == DateTimeKind.Unspecified)
                return new DateTime(DateTime.MaxValue.Ticks, DateTimeKind.Utc);
            return new DateTime(DateTime.MaxValue.Ticks, kind);
        }

        /// <summary>
        /// Gets the Minimum value for a DateTime specifying kind.
        /// </summary>
        /// <param name="kind">DateTimeKind to use.</param>
        /// <returns>DateTime of specified kind.</returns>
        public static DateTime GetMinValue(DateTimeKind kind)
        {
            if (kind == DateTimeKind.Unspecified)
                return new DateTime(DateTime.MinValue.Ticks, DateTimeKind.Utc);
            return new DateTime(DateTime.MinValue.Ticks, kind);
        }
    }
}