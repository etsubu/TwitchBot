using System;
using System.Collections.Generic;
using System.Text;
using TwitchBot.Commands;

namespace TwitchBot
{
    internal class Channel
    {
        private readonly string name;
        private CommandHandler commandHandler;
        private Action<ChatMessage> chatListener;

        public Channel(IRC irc, string name)
        {
            this.name = name;
            this.chatListener = MessageReceived;
            this.commandHandler = new CommandHandler(irc, name);
            irc.RegisterMessageCallback(this.MessageReceived, this.name);
        }

        public void MessageReceived(ChatMessage message)
        {
            commandHandler.ProcessCommand(message.Message, message.Sender, message.Channel);
        }
    }
}
