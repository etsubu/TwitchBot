using System;
using System.Collections.Generic;
using System.Text;
using TwitchBot.Commands;

namespace TwitchBot
{
    class ChatBot
    {
        private IRC irc;
        private CommandHandler commands;

        /// <summary>
        /// Called when the an IRC message is received
        /// </summary>
        /// <param name="message">ChatMessage that contains the parsed message</param>
        public void MessageReceived(ChatMessage message)
        {
            Console.WriteLine(message.GetCommand());
            
            if(message.GetCommand().Equals("PRIVMSG"))
            {
                //Console.WriteLine(message.);
                string line = message.GetTrailing();
                this.commands.ProcessCommand(line);
            }
        }

        /// <summary>
        /// Initializes ChatBot
        /// </summary>
        public ChatBot()
        {
            this.irc = new IRC();
            this.commands = new CommandHandler(this.irc);
            this.irc.MessageReceivedEvent += MessageReceived;
            this.irc.ConnectServer("irc.twitch.tv", 6667, "nagrodusbot", /*OAUTH KEY HERE*/"");
            this.irc.JoinChannel("#nagrodus");
            this.irc.SendMessage("Hello world", "#nagrodus");
        }
    }
}
