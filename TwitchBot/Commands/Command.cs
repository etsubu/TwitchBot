﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Defines a single command
    /// </summary>
    internal abstract class Command
    {
        /// <summary>
        /// Used to check if the command can be removed by the user
        /// </summary>
        /// <returns>True if the command can be removed, false if not</returns>
        public abstract bool IsRemoveable { get; }

        /// <summary>
        /// The name of this command
        /// </summary>
        /// <returns>Returns the identifier part for the command</returns>
        public string Name => name;

        private readonly string name;
        private readonly int requiredPermission;
        protected readonly CommandHandler Handler;

        /// <summary>
        /// Initializes the Command 
        /// </summary>
        /// <param name="name">The identifier of the command</param>
        public Command(CommandHandler handler, string name) : this(handler, name, 0)
        {
        }

        /// <summary>
        /// Initializes the Command
        /// </summary>
        /// <param name="name">The identifier of the command</param>
        /// <param name="requiredPermission">Required permission level for the command</param>
        public Command(CommandHandler handler, string name, int requiredPermission)
        {
            Handler = handler;
            this.name = name;
            this.requiredPermission = requiredPermission;
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
        /// Checks if the given permission level is enough to use this command
        /// </summary>
        /// <param name="permission">Permission level to check against</param>
        /// <returns>True if the given permission is enough, false if not</returns>
        public bool HasPermission(int permission) => permission >= requiredPermission;

        /// <summary>
        /// Processes the command
        /// </summary>
        /// <param name="line">Command line to process</param>
        /// <param name="permission">Permission level of the sender</param>
        /// <param name="sender">Sender name</param>
        /// <returns>String to send as a response</returns>
        public abstract string Process(string line, string sender);
    }
}
