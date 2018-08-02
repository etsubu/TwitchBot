using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot.Extensions
{
    internal static class TimeSpanExtensions
    {
        public static string ToFriendlyString(long secondsTotal)
        {
            var hours = secondsTotal / 3600;
            var minutes = (secondsTotal % 3600) / 60;
            var seconds = ((secondsTotal % 3600) % 60);
            var stringBuilder = new StringBuilder();

            if (hours > 0)
                stringBuilder.Append($"{hours} hours, ");

            if (minutes > 0)
                stringBuilder.Append($"{minutes} minutes, ");

            stringBuilder.Append($"{seconds} seconds");
            return stringBuilder.ToString();
        }
    }
}
