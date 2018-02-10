using System;
using System.Collections.Generic;
using System.Text;
using TwitchBot.Extensions;

namespace TwitchBot.Commands
{
    /// <summary>
    /// UptimeCommand is used to retrieve the amount of time the program has been running in hours, minutes and seconds
    /// </summary>
    internal class UptimeCommand : Command
    {
        /// <summary>
        /// UptimeCommand is not removeable
        /// </summary>
        /// <returns>False</returns>
        public override bool IsRemoveable => false;

        private readonly DateTimeOffset startTime;

        /// <summary>
        /// Initializes UptimeCommand
        /// </summary>
        public UptimeCommand() : base("uptime")
        {
            startTime = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Calculates the time this program has been running
        /// </summary>
        /// <param name="line"></param>
        /// <returns>Running time in hours, minutes and seconds</returns>
        public override string Process(string line) => (DateTimeOffset.UtcNow - startTime).ToFriendlyString();
    }
}
