using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot.Commands
{
    /// <summary>
    /// MetaCommand handles modifying, adding, removing and listing of the existing commands
    /// </summary>
    internal class MetaCommand : Command
    {
        /// <summary>
        /// MetaCommand cannot be removed by users
        /// </summary>
        /// <returns>False</returns>
        public override bool IsRemoveable => false;

        /// <summary>
        /// MetaCommand is not global command
        /// </summary>
        public override bool IsGlobal => false;

        private CommandHandler Handler;
        /// <summary>
        /// Initializes MetaCommand
        /// </summary>
        public MetaCommand(CommandHandler handler) : base("command", 1)
        {
            Handler = handler;
        }

        /// <summary>
        /// Processes the given line
        /// </summary>
        /// <param name="line">Line to process</param>
        /// <param name="sender">sender name</param>
        /// <returns></returns>
        public override CommandResult Process(string line, string sender, string botname)
        {
            string[] parts = line.Split(" ");
            if (parts.Length < 2)
                return new CommandResult(false, "Invalid command.");

            if (parts[1].Equals("list"))
                return new CommandResult(true, Handler.ListCommands());

            if (parts[1].Equals("add") && parts.Length > 3)
            {
                if (Handler.AddCommand(parts[2], string.Join(' ', parts, 3, parts.Length - 3)))
                    return new CommandResult(true, "Command was successfully added.");

                return new CommandResult(false, "Failed to add command.");
            }

            if (parts[1].Equals("remove") && parts.Length == 3)
            {
                if (Handler.RemoveCommand(parts[2]))
                    return new CommandResult(true, "Command was successfully removed.");

                return new CommandResult(false, "Could not remove command.");
            }

            return new CommandResult(false, "Invalid command.");
        }

        /// <summary>
        /// Implements the help interface
        /// </summary>
        /// <returns>Help text</returns>
        public override string Help()
        {
            return "Usage: !command list --- !command add [COMMAND_NAME] [COMMAND_RESPONSE] --- !command remove [COMMAND_NAME]";
        }
    }
}
