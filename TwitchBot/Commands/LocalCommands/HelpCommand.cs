using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot.Commands.LocalCommands
{
    /// <summary>
    /// Retrieves the help messages of other commands
    /// </summary>
    internal class HelpCommand : Command
    {
        public override bool IsRemoveable => false;
        public override bool IsGlobal => false;
        private CommandHandler Handler;

        /// <summary>
        /// Initializes HelpCommand
        /// </summary>
        /// <param name="handler"></param>
        public HelpCommand(CommandHandler handler) : base("help")
        {
            Handler = handler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the help text</returns>
        public override string Help()
        {
            return "Use this to retrieve help message for different commands. !help CommandName";
        }

        /// <summary>
        /// Parses the command name to retrieve help text for. If none was given returns this commands help text
        /// </summary>
        /// <param name="line">Command given</param>
        /// <param name="sender">Name of the sender</param>
        /// <returns></returns>
        public override CommandResult Process(string line, string sender, string botname)
        {
            string[] parts = line.Split(" ");
            if (parts.Length == 1)
                return new CommandResult(true, Help());
            return Handler.GetHelp(parts[1]);
        }
    }
}
