using System;

namespace TwitchBot
{
    internal class ChatBot : IDisposable
    {
        private readonly IRC irc;

        /// <summary>
        /// Initializes ChatBot
        /// </summary>
        public ChatBot()
        {
            irc = new IRC();
            irc.MessageReceivedEvent += MessageReceived;
            irc.ConnectServer("irc.twitch.tv", 6667, "nagrodusbot", /*OAUTH KEY HERE*/"");
            irc.JoinChannel("#nagrodus");
            irc.SendMessage("Hello world", "#nagrodus");
        }

        /// <summary>
        /// Called when the an IRC message is received
        /// </summary>
        /// <param name="message">ChatMessage that contains the parsed message</param>
        public void MessageReceived(ChatMessage message)
        {
            Console.WriteLine(message.Command);
        }

        /// <summary>
        /// Synchronously waits for the IRC client to exit
        /// </summary>
        public void WaitForExit() => irc.WaitForExit();

        public void Dispose()
        {
            irc?.Dispose();
        }
    }
}
