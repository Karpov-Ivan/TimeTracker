using System;
using System.Text.RegularExpressions;

namespace TimeTracker.Common
{
    public class TimeConverter
    {
        private const int SecondsPerMinute = 60;
        private const int MinutesPerHour = 60;
        private const int HoursPerDay = 24;
        private const int DaysPerWeek = 7;
        private const int SecondsPerDay = SecondsPerMinute * MinutesPerHour * HoursPerDay;
        private const int SecondsPerWeek = SecondsPerDay * DaysPerWeek;
        private const int SecondsPerMonth = SecondsPerDay * 30;

        public static long GetTimeInSeconds(string timeString)
        {
            var regex = new Regex(@"(\d+)([a-zA-Z]+)");
            var matches = regex.Matches(timeString);

            long totalTimeInSeconds = 0;

            foreach (Match match in matches)
            {
                var value = int.Parse(match.Groups[1].Value);
                var unit = match.Groups[2].Value.ToLower();

                totalTimeInSeconds += unit switch
                {
                    "mo" => value * SecondsPerMonth,
                    "w" => value * SecondsPerWeek,
                    "d" => value * SecondsPerDay,
                    "h" => value * MinutesPerHour * SecondsPerMinute,
                    "m" => value * SecondsPerMinute,
                    "s" => value,
                    _ => throw new NotImplementedException()
                };
            }

            if (timeString.StartsWith("subtracted", StringComparison.OrdinalIgnoreCase))
            {
                totalTimeInSeconds *= -1;
            }

            return totalTimeInSeconds;
        }
    }
}