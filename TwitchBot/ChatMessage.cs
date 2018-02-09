using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot
{
    /// <summary>
    /// Contains a single IRC message
    /// </summary>
    class ChatMessage
    {
        private string prefix, trailing, command, username;
        private string[] parameters;

        /// <summary>
        /// Initializes the ChatMessage
        /// </summary>
        /// <param name="prefix">Prefix of the message (optional)</param>
        /// <param name="command">Command of the received message</param>
        /// <param name="trailing">Trailing part of the message (optional)</param>
        /// <param name="parameters">Parameters of the message (optional)</param>
        public ChatMessage(string prefix, string command, string trailing, string[] parameters)
        {
            this.prefix = prefix;
            this.command = command;
            this.trailing = trailing;
            this.parameters = parameters;
            this.username = null;
            if(command.Equals("PRIVMSG") && prefix.Length > 1 && prefix[0] == ':')
            {
                int nameEndIndex = prefix.IndexOf("!");
                if(nameEndIndex != -1)
                {
                    this.username = prefix.Substring(1, nameEndIndex - 1);
                }
            }
        }

        /// <summary>
        /// Returns the name of the user who sent the message if available. This value can be null if the information was unavailable
        /// </summary>
        /// <returns>Name of the sender or null if it the info doesn't exist</returns>
        public string GetUsername()
        {
            return this.username;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the prefix</returns>
        public string GetPrefix()
        {
            return this.prefix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the trailing part</returns>
        public string GetTrailing()
        {
            return this.trailing;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the command</returns>
        public string GetCommand()
        {
            return this.command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the parameters</returns>
        public string[] GetParameters()
        {
            return this.parameters;
        }
    }
}
