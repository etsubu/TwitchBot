using System;
using TwitchBot.Commands;

namespace TwitchBot
{
    internal class ChatBot : IDisposable
    {
        private readonly IRC irc;
        private readonly CommandHandler commands;
        private readonly Channel[] channels;

        /// <summary>
        /// Initializes ChatBot
        /// </summary>
        private ChatBot()
        {
            irc = new IRC();
        }

        public ChatBot(Configuration configuration) : this()
        {
            irc.ConnectServer(
                configuration.Connection.Host,
                configuration.Connection.Port,
                configuration.Username,
                configuration.OAuthToken);
            channels = new Channel[configuration.Channels.Length];

            for(int i = 0; i < configuration.Channels.Length; i++)
            {
                irc.JoinChannel(configuration.Channels[i]);
                channels[i] = new Channel(irc, configuration.Channels[i]);
            }
        }

        /// <summary>
        /// Called when the an IRC message is received
        /// </summary>
        /// <param name="message">ChatMessage that contains the parsed message</param>
        public void MessageReceived(ChatMessage message)
        {

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
