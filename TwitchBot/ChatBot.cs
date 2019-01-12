using System;
using System.Collections.Generic;
using System.Threading;
using TwitchBot.Commands;
using TwitchBot.Commands.Permissions;

namespace TwitchBot
{
    /// <summary>
    /// ChatBot handles login and behaviour for one account that can be active on multiple channels
    /// </summary>
    internal class ChatBot : IDisposable
    {
        private readonly IRC irc;
        private List<Channel> Channels;
        private readonly GlobalCommand globalCommand;
        private readonly PermissionManager permissionManager;
        private Database database;
        private Configuration configuration;

        /// <summary>
        /// Initializes ChatBot
        /// </summary>
        /// <param name="configuration">Configuration for the login</param>
        /// <param name="database">Database instance for synchronizing data with</param>
        public ChatBot(Configuration configuration, Database database)
        {
            this.database = database;
            this.configuration = configuration;
            ChannelName ownChannelName = new ChannelName(configuration.Username);
            irc = new IRC();
            while(!irc.ConnectServer(
                configuration.Connection.Host,
                configuration.Connection.Port,
                configuration.Username,
                configuration.OAuthToken))
            {
                Console.WriteLine("Sleeping 2 seconds and reconnecting...");
                Thread.Sleep(2000);
            }
            Channels = new List<Channel>();
            permissionManager = new PermissionManager(database, configuration.Owner);
            globalCommand = new GlobalCommand(this);

            irc.JoinChannel(ownChannelName);
            Channel ownChannel = new Channel(irc, ownChannelName, globalCommand, permissionManager, database);
            Channels.Add(ownChannel);

            List<string> channelNames = database.QueryChannels();
            foreach (string channelName in channelNames)
            {
                ChannelName channel = new ChannelName(channelName);
                irc.JoinChannel(channel);
                Channels.Add(new Channel(irc, channel, globalCommand, permissionManager, database));
            }
        }

        /// <summary>
        /// Broadcasts a global message to all channel currently joined
        /// </summary>
        /// <param name="message">Message to be broadcasted</param>
        public void BroadcastToChannels(string message)
        {
            lock(this.Channels)
            {
                foreach (Channel channel in Channels)
                {
                    irc.SendMessage("[GLOBAL]: " + message, channel.Name);
                }
            }
        }

        /// <summary>
        /// Joins a new channel
        /// </summary>
        /// <param name="username">User whose channel to join</param>
        public void JoinToChannel(string username)
        {
            lock(this.Channels)
            {
                ChannelName channelName = new ChannelName(username);
                foreach (Channel channel in Channels)
                {
                    if (channel.Name.Equals(channelName))
                        return;
                }
                if (database.AddChannel(channelName))
                {
                    irc.JoinChannel(channelName);
                    Channels.Add(new Channel(irc, channelName, globalCommand, permissionManager, database));
                    irc.SendMessage("Joined channel KappaClaus", channelName);
                }
            }
        }

        /// <summary>
        /// Leaves the given channel and removes listeners associated with it
        /// </summary>
        /// <param name="username">User whose channel to leave</param>
        /// <returns>True if bot was joined and has now left, false if it wasn't in the channel</returns>
        public bool LeaveChannel(ChannelName channel)
        {
            lock(this.Channels)
            {
                for(int i = 0; i < Channels.Count; i++)
                {
                    if (channel.Equals(Channels[i].Name))
                    {
                        irc.SendMessage("Leaving channel... bye bye BibleThump");
                        Channels.RemoveAt(i);
                        irc.LeaveChannel(channel);
                        return database.RemoveChannel(channel);
                    }
                }
            }
            return false;
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
