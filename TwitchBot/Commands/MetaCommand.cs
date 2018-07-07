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
        /// Initializes MetaCommand
        /// </summary>
        public MetaCommand(CommandHandler handler) : base(handler, "command", 1)
        {
        }

        /// <summary>
        /// Processes the given line
        /// </summary>
        /// <param name="line">Line to process</param>
        /// <param name="sender">sender name</param>
        /// <returns></returns>
        public override CommandResult Process(string line, string channel, string sender)
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
