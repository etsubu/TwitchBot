using System;
using System.Collections.Generic;
using System.Text;
using TwitchBot.Commands;

namespace TwitchBot
{
    /// <summary>
    /// Handles the activity inside a single channel. Isolated unit
    /// </summary>
    internal class Channel
    {
        private readonly string name;
        private CommandHandler commandHandler;
        private Action<ChatMessage> chatListener;

        /// <summary>
        /// Initializes Channel
        /// </summary>
        /// <param name="irc"></param>
        /// <param name="name"></param>
        public Channel(IRC irc, string name)
        {
            this.name = name;
            this.chatListener = MessageReceived;
            this.commandHandler = new CommandHandler(irc, name);
            irc.RegisterMessageCallback(this.MessageReceived, this.name);
        }

        /// <summary>
        /// Callback function that is called when a message is received inside this channel
        /// </summary>
        /// <param name="message"></param>
        public void MessageReceived(ChatMessage message)
        {
            commandHandler.ProcessCommand(message.Message, message.Sender, message.Channel);
        }
    }
}
