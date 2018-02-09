using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot
{
    /// <summary>
    /// BasicCommand defines a simple Command-Response behaviour where the response value is static
    /// </summary>
    class BasicCommand : Command
    {
        private string response;

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
        /// <returns>Static response to the command or null if the line does not belong to this command</returns>
        public override string Process(string line)
        {
            if(BelongsTo(line))
                return this.response;
            return null;
        }
    }
}
