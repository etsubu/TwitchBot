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
        private string prefix, trailing, command;
        private string[] parameters;

        public ChatMessage(string prefix, string command, string trailing, string[] parameters)
        {
            this.prefix = prefix;
            this.command = command;
            this.trailing = trailing;
            this.parameters = parameters;
        }
    }
}
