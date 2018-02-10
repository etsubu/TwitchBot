using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Defines a single command
    /// </summary>
    internal abstract class Command
    {
        private readonly string name;
        private readonly int requiredPermission;

        /// <summary>
        /// Initializes the Command 
        /// </summary>
        /// <param name="name">The identifier of the command</param>
        public Command(string name)
        {
            this.name = name;
            requiredPermission = 0;
        }

        /// <summary>
        /// Initializes the Command
        /// </summary>
        /// <param name="name">The identifier of the command</param>
        /// <param name="requiredPermission">Required permission level for the command</param>
        public Command(string name, int requiredPermission)
        {
            this.name = name;
            this.requiredPermission = requiredPermission;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the identifier part for the command</returns>
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Checks if the given command belongs to this command
        /// </summary>
        /// <param name="line">Command line</param>
        /// <returns>True if the command belongs to this command, false if not</returns>
        public bool BelongsTo(string line)
        {
            if (line.Length > 1)
                return line.StartsWith(line);

            return false;
        }

        /// <summary>
        /// Processes the command
        /// </summary>
        /// <param name="line">Command line to process</param>
        /// <returns>String to send as a response</returns>
        public abstract string Process(string line);
    }
}
