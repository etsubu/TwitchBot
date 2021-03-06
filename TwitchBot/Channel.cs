﻿using System;
using TwitchBot.Commands;
using TwitchBot.Commands.MessageFilters;
using TwitchBot.Commands.Permissions;

namespace TwitchBot
{
    /// <summary>
    /// Handles the activity inside a single channel. Isolated unit
    /// </summary>
    internal class Channel
    {
        public readonly ChannelName Name;
        private CommandHandler commandHandler;
        private MessageFilterHandler filters;
        private Action<ChatMessage> chatListener;
        public readonly IRC Irc;

        /// <summary>
        /// Initializes Channel
        /// </summary>
        /// <param name="irc"></param>
        /// <param name="name"></param>
        /// <param name="database">Database object to synchronize channel settings with</param>
        public Channel(IRC irc, ChannelName name, GlobalCommand globalCommand, PermissionManager permissionManager, Database database)
        {
            Irc = irc;
            Name = name;
            chatListener = MessageReceived;
            this.filters = new MessageFilterHandler(permissionManager, irc);
            commandHandler = new CommandHandler(irc, filters, name, globalCommand, permissionManager, database);
            irc.RegisterMessageCallback(this.MessageReceived, Name);
        }

        /// <summary>
        /// Callback function that is called when a message is received inside this channel
        /// </summary>
        /// <param name="message"></param>
        public void MessageReceived(ChatMessage message)
        {
            Console.WriteLine(message.Message);
            if(!commandHandler.ProcessCommand(message.Message, message.Sender, message.Channel))
            {
                // Only invoke filters if given message was not command
                Console.WriteLine("Invoke filters");
                filters.ProcessFilters(message, Irc.user);
            }
        }
    }
}
