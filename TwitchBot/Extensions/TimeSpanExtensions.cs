using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot.Extensions
{
    internal static class TimeSpanExtensions
    {
        // TODO: not tested
        public static string ToFriendlyString(this TimeSpan timeSpan)
        {
            var hours = timeSpan.Seconds / 3600;
            var minutes = (timeSpan.Seconds % 3600) / 60;
            var seconds = ((timeSpan.Seconds % 3600) % 60) / 60;
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
