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

        /// <summary>
        /// UptimeCommand is not global command
        /// </summary>
        public override bool IsGlobal => false;

        private DateTimeOffset startTime;

        /// <summary>
        /// Initializes UptimeCommand
        /// <param name="handler">Ununsed by the UptimeCommand</param>
        /// </summary>
        public UptimeCommand() : base("uptime")
        {
            startTime = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Calculates the time this program has been running or resets the timer
        /// </summary>
        /// <param name="line">Command line</param>
        /// <returns>Running time in hours, minutes and seconds or a message indicating that the timer has been reset</returns>
        public override CommandResult Process(string line, string sender, string botname)
        {
            if(line.Equals("uptime"))
            {
                return new CommandResult(true, TimeSpanExtensions.ToFriendlyString(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - startTime.ToUnixTimeSeconds()));
            }
            else if(line.Equals("uptime reset"))
            {
                startTime = DateTimeOffset.UtcNow;
                return new CommandResult(true, "Uptime has been reset");
            } else
            {
                return new CommandResult(false, "Unknown command");
            }
        }

        /// <summary>
        /// Implements help interface
        /// </summary>
        /// <returns></returns>
        public override string Help()
        {
            return "Usage: !uptime --- !uptime reset";
        }
    }
}
