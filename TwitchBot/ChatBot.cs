using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot
{
    class ChatBot
    {
        private IRC irc;

        public void MessageReceived(ChatMessage message)
        {
            Console.WriteLine(message.GetCommand());
           /* if(message.GetCommand() == "376")
            {
                this.irc.JoinChannel("#nagrodus");
                
            }*/
            if(message.GetCommand() == "366")
            {
                Console.WriteLine("hello");
                this.irc.SendMessage("Hello world", "#nagrodus");
            }
        }

        public ChatBot()
        {
            this.irc = new IRC();
            this.irc.MessageReceivedEvent += MessageReceived;
            this.irc.ConnectServer("irc.twitch.tv", 6667, "nagrodusbot", "");
            this.irc.JoinChannel("#nagrodus");
        }
    }
}
