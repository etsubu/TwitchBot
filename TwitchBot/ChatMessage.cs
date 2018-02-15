using System;

namespace TwitchBot
{
    /// <summary>
    /// Contains a single IRC PRIVMSG message
    /// </summary>
    internal class ChatMessage
    {
        public string Sender { get; }
        public string Channel { get; }
        public string Message { get; }

        /// <summary>
        /// Initializes ChatMessage from IRCMessage with command type PRIVMSG. Will fail if the IRCMessage is not PRIVMSG
        /// </summary>
        /// <param name="message"></param>
        public ChatMessage(IRCMessage message)
        {
            if (!message.Equals("PRIVMSG"))
                throw new ArgumentException();
            int nameEndIndex = message.Prefix.IndexOf("!", StringComparison.Ordinal);
            if (nameEndIndex == -1 || nameEndIndex == 0)
                throw new ArgumentException();
            Sender = message.Prefix.Substring(0, nameEndIndex);
            if (message.Parameters == null)
                throw new ArgumentException();
            Channel = message.Parameters[0];
            Message = message.Trailing;
        }
    }
}
