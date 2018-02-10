using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot
{
    /// <summary>
    /// MetaCommand handles modifying, adding, removing and listing of the existing commands
    /// </summary>
    class MetaCommand:Command
    {
        private CommandHandler handler;

        /// <summary>
        /// Initializes MetaCommand
        /// </summary>
        public MetaCommand(CommandHandler handler):base("command", 1)
        {
            this.handler = handler;
        }

        /// <summary>
        /// Processes the given line
        /// </summary>
        /// <param name="line">Line to process</param>
        /// <param name="sender">sender name</param>
        /// <returns></returns>
        public override string Process(string line, string sender)
        {
            string[] parts = line.Split(" ");
            if (parts.Length < 2)
                return "Invalid command.";
            if(parts[1].Equals("list"))
            {
                return this.handler.ListCommands();
            }
            else if(parts[1].Equals("add") && parts.Length > 3)
            {
                if (this.handler.AddCommand(parts[2], string.Join(' ', parts, 3, parts.Length - 3)))
                    return "Command was successfully added.";
                else
                    return "Failed to add command.";
            } else if(parts[1].Equals("remove") && parts.Length == 3)
            {
                if (this.handler.RemoveCommand(parts[2]))
                    return "Command was successfully removed.";
                else
                    return "Could not remove command.";
            }
            return "Invalid command.";
        }

        /// <summary>
        /// MetaCommand cannot be removed by users
        /// </summary>
        /// <returns>False</returns>
        public override bool IsRemoveable()
        {
            return false;
        }
    }
}
