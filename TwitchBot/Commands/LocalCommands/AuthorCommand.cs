using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot.Commands.LocalCommands
{
    /// <summary>
    /// AuthorCommand can be used to retrieve authors of the program and link to the version control
    /// </summary>
    internal class AuthorCommand : Command
    {
        public override bool IsRemoveable => false;

        public override bool IsGlobal => false;

        /// <summary>
        /// Initializes AuthorCommand
        /// </summary>
        public AuthorCommand() : base("author")
        {

        }

        /// <summary>
        /// Retrieves help text
        /// </summary>
        /// <returns>Help text of this command</returns>
        public override string Help()
        {
            return "Usage is !author to display the authors of this program and the link to version control";
        }

        /// <summary>
        /// No conditions. returns authors of the program
        /// </summary>
        /// <param name="line"></param>
        /// <param name="sender"></param>
        /// <returns>Authors of the program and link to version control</returns>
        public override CommandResult Process(string line, string sender)
        {
            return new CommandResult(true, "Authors: nagrodus/etsubu and Spans. https://github.com/etsubu/TwitchBot FrankerZ");
        }
    }
}
