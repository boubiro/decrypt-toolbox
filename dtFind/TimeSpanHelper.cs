using System;

namespace dtFind
{
    static class TimeSpanHelper
    {
        public static string ToReadableString(this TimeSpan span)
        {
            var timeSpan = span.Duration();

            if (timeSpan.TotalMinutes < 1)
            {
                return String.Format("{0:0} seconds", timeSpan.Seconds);
            }
            else
            {
                string formatted = String.Format("{0}{1}{2}",
                                                 timeSpan.Days > 0 ? String.Format("{0:0} days, ", timeSpan.Days) : String.Empty,
                                                 timeSpan.Hours > 0 ? String.Format("{0:0} hours, ", timeSpan.Hours) : String.Empty,
                                                 timeSpan.Minutes > 0 ? String.Format("{0:0} minutes, ", timeSpan.Minutes) : String.Empty);

                return formatted.Substring(0, formatted.Length - 2);
            }
        }
    }
}