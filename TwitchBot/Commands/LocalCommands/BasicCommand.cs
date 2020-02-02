using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot.Commands
{
    /// <summary>
    /// BasicCommand defines a simple Command-Response behaviour where the response value is static
    /// </summary>
    internal class BasicCommand : Command
    {
        /// <summary>
        /// BasicCommand can be removed by the user
        /// </summary>
        /// <returns>True</returns>
        public override bool IsRemoveable => true;

        /// <summary>
        /// BasicCommand is not global command
        /// </summary>
        public override bool IsGlobal => false;

        private readonly string response;

        /// <summary>
        /// Initializes the BasicCommand
        /// </summary>
        /// <param name="name">Name/Identifier that belongs to this command</param>
        /// <param name="response">Statis response when this command is called</param>
        public BasicCommand(string name, string response) : base(name)
        {
            this.response = response;
        }

        /// <summary>
        /// Checks if the line belongs to this command and processes it if it does
        /// </summary>
        /// <param name="line">Command line to process</param>
        /// <param name="sender">sender name</param>
        /// <returns>Static response to the command</returns>
        public override CommandResult Process(string line, string sender, string botname)
        {
            return new CommandResult(true, this.response);
        }

        /// <summary>
        /// Implement help interface
        /// </summary>
        /// <returns>Help text</returns>
        public override string Help()
        {
            return "This a command with static response. Use " + Name;
        }
    }
}
