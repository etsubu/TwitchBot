using System;
using System.Collections.Generic;
using System.Text;
using TwitchBot.Commands;
using TwitchBot.Commands.Permissions;

namespace TwitchBot
{
    /// <summary>
    /// Handles the activity inside a single channel. Isolated unit
    /// </summary>
    internal class Channel
    {
        public readonly string Name;
        private CommandHandler commandHandler;
        private Action<ChatMessage> chatListener;

        /// <summary>
        /// Initializes Channel
        /// </summary>
        /// <param name="irc"></param>
        /// <param name="name"></param>
        public Channel(IRC irc, string name, GlobalCommand globalCommand, PermissionManager permissionManager)
        {
            Name = name;
            chatListener = MessageReceived;
            commandHandler = new CommandHandler(irc, name, globalCommand, permissionManager);
            irc.RegisterMessageCallback(this.MessageReceived, Name);
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
