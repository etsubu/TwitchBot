using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot
{
    /// <summary>
    /// UptimeCommand is used to retrieve the amount of time the program has been running in hours, minutes and seconds
    /// </summary>
    class UptimeCommand : Command
    {
        private int startTime;

        /// <summary>
        /// Initializes UptimeCommand
        /// </summary>
        public UptimeCommand():base("uptime")
        {
            this.startTime = Environment.TickCount;
        }

        /// <summary>
        /// UptimeCommand is not removeable
        /// </summary>
        /// <returns>False</returns>
        public override bool IsRemoveable()
        {
            return false;
        }

        /// <summary>
        /// Calculates the time this program has been running
        /// </summary>
        /// <param name="line"></param>
        /// <returns>Running time in hours, minutes and seconds</returns>
        public override string Process(string line)
        {
            string uptime;
            int diff = (Environment.TickCount - this.startTime) / 1000;
            if(diff > 60 * 60)
            {
                int hours = diff / (60 * 60);
                diff = diff % (60 * 60);
                int minutes = (diff / 60);
                int seconds = (diff % 60);
                uptime = "Up for " + hours + " hours " + minutes + " minutes " + seconds + " seconds";
            } else if(diff > 60)
            {
                int minutes = (diff / 60);
                int seconds = (diff % 60);
                uptime = "Up for " + minutes + " minutes " + seconds + " seconds";
            } else
            {
                uptime = "Up for " + diff + " seconds";
            }
            return uptime;
        }
    }
}
