using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Contains the info whether a command succeeded or not and response message to be sent to the user
    /// </summary>
    class CommandResult
    {
        public bool Succeeded { get; }
        public string Response { get; }

        /// <summary>
        /// Initializes CommandResult
        /// </summary>
        /// <param name="succeeded">True if the command succeeded, false if failed</param>
        /// <param name="response">The response of the command to the user as text</param>
        public CommandResult(bool succeeded, string response)
        {
            this.Succeeded = succeeded;
            this.Response = response;
        }
    }
}
