using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot
{
    class ChatBot
    {
        private IRC irc;

        public ChatBot()
        {
            this.irc = new IRC();
            this.irc.ConnectServer("irc.twitch.tv", 6667);
        }
    }
}
