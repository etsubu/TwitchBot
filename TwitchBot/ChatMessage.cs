namespace TwitchBot
{
    /// <summary>
    /// Contains a single IRC message
    /// </summary>
    internal class ChatMessage
    {
        /// <summary>
        /// Prefix of the message
        /// </summary>
        /// <returns>Returns the prefix</returns>
        public string Prefix { get; }

        /// <summary>
        /// Trailing part of the message
        /// </summary>
        /// <returns>Returns the trailing part</returns>
        public string Trailing { get; }

        /// <summary>
        /// Command of the message
        /// </summary>
        /// <returns>Returns the command</returns>
        public string Command { get; }

        /// <summary>
        /// Parameters of the message
        /// </summary>
        /// <returns>Returns the parameters</returns>
        public string[] Parameters { get; }

        /// <summary>
        /// Initializes the ChatMessage
        /// </summary>
        /// <param name="prefix">Prefix of the message (optional)</param>
        /// <param name="command">Command of the received message</param>
        /// <param name="trailing">Trailing part of the message (optional)</param>
        /// <param name="parameters">Parameters of the message (optional)</param>
        public ChatMessage(string prefix, string command, string trailing, string[] parameters)
        {
            Prefix = prefix;
            Command = command;
            Trailing = trailing;
            Parameters = parameters;
        }
    }
}
