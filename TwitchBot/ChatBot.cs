using System;

namespace TwitchBot
{
    internal class ChatBot : IDisposable
    {
        private readonly IRC irc;

        /// <summary>
        /// Initializes ChatBot
        /// </summary>
        private ChatBot()
        {
            irc = new IRC();
            irc.MessageReceivedEvent += MessageReceived;
        }

        public ChatBot(Configuration configuration) : this()
        {
            irc.ConnectServer(
                configuration.Connection.Host,
                configuration.Connection.Port,
                configuration.Username,
                configuration.OAuthToken);

            foreach (var channel in configuration.Channels)
            {
                irc.JoinChannel(channel);
                irc.SendMessage("Hello world", channel);
            }
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
