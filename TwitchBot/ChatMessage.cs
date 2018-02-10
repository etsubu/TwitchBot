﻿namespace TwitchBot
{
    /// <summary>
    /// Contains a single IRC message
    /// </summary>
    class ChatMessage
    {
        private readonly string prefix;
        private readonly string trailing;
        private readonly string command;
        private readonly string[] parameters;

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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the prefix</returns>
        public string GetPrefix()
        {
            return prefix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the trailing part</returns>
        public string GetTrailing()
        {
            return trailing;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the command</returns>
        public string GetCommand()
        {
            return command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the parameters</returns>
        public string[] GetParameters()
        {
            return parameters;
        }
    }
}
