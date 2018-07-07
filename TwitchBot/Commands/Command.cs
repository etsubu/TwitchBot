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
        /// <summary>
        /// Used for checking if the command can be removed by the user
        /// </summary>
        /// <returns>True if the command can be removed, false if not</returns>
        public abstract bool IsRemoveable { get; }

        /// <summary>
        /// Used for checking if the command is global
        /// </summary>
        public abstract bool IsGlobal { get; }

        /// <summary>
        /// The name of this command
        /// </summary>
        /// <returns>Returns the identifier part for the command</returns>
        public string Name => name;

        private readonly string name;
        private readonly int RequiredPermission;
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
            this.RequiredPermission = requiredPermission;
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
        public bool HasPermission(int permission) => permission >= RequiredPermission;

        /// <summary>
        /// Processes the command
        /// </summary>
        /// <param name="line">Command line to process</param>
        /// <param name="sender">Sender name</param>
        /// <returns>CommandResult object containing the information about the result of the executed command</returns>
        public abstract CommandResult Process(string line, string sender);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Help text for the usage of the command</returns>
        public abstract string Help();
    }
}
