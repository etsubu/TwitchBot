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
        public MetaCommand(CommandHandler handler):base("command")
        {
            this.handler = handler;
        }

        private void ListCommands()
        {

        }

        /// <summary>
        /// Processes the given line
        /// </summary>
        /// <param name="line">Line to process</param>
        /// <returns></returns>
        public override string Process(string line)
        {
            string[] parts = line.Split(" ");
            if (parts.Length < 2)
                return "";
            if(parts[1].Equals("list"))
            {
                ListCommands();
            }
            else if(parts[1].Equals("add") && parts.Length >2)
            {

            } else if(parts[1].Equals("remove") && parts.Length == 3)
            {

            }
            return "";
        }
    }
}
