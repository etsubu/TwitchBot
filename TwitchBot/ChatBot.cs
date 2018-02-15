using System;
using TwitchBot.Commands;

namespace TwitchBot
{
    internal class ChatBot
    {
        private readonly IRC irc;
        private readonly Configuration configuration;
        private readonly CommandHandler commandHandler;

        /// <summary>
        /// Initializes ChatBot
        /// </summary>
        public ChatBot(IRC irc, Configuration configuration, CommandHandler commandHandler)
        {
            this.irc = irc;
            this.configuration = configuration;
            this.commandHandler = commandHandler;

            this.irc.MessageReceivedEvent += MessageReceived;
        }

        public void Start()
        {
            irc.ConnectServer(
                configuration.Connection.Host,
                configuration.Connection.Port,
                configuration.Username,
                configuration.OAuthToken);

            foreach (var channel in configuration.Channels)
            {
                // TODO: register channel-specific callback for IRC
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
            
            if (message.Command.Equals("PRIVMSG") && message.Username != null)
            {
                //Console.WriteLine(message.);
                string line = message.Trailing;
                commandHandler.ProcessCommand(line, message.Username.ToLower(), message.Parameters[0]);
            }
        }

        /// <summary>
        /// Synchronously waits for the IRC client to exit
        /// </summary>
        public void WaitForExit() => irc.WaitForExit();
    }
}
