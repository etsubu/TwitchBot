using System;
using System.Collections.Generic;
using TwitchBot.Commands;
using TwitchBot.Commands.Permissions;

namespace TwitchBot
{
    internal class ChatBot : IDisposable
    {
        private readonly IRC irc;
        private List<Channel> Channels;
        private GlobalCommand globalCommand;
        private PermissionManager permissionManager;

        public ChatBot(Configuration configuration)
        {
            irc = new IRC();
            irc.ConnectServer(
                configuration.Connection.Host,
                configuration.Connection.Port,
                configuration.Username,
                configuration.OAuthToken);
            Channels = new List<Channel>();
            permissionManager = new PermissionManager();
            globalCommand = new GlobalCommand(this);

            Channel ownChannel = new Channel(irc, configuration.Username, globalCommand, permissionManager);
            Channels.Add(ownChannel);
            irc.JoinChannel(configuration.Username);

            for (int i = 0; i < configuration.Channels.Length; i++)
            {
                irc.JoinChannel(configuration.Channels[i]);
                Channels.Add(new Channel(irc, configuration.Channels[i], globalCommand, permissionManager));
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
                irc.JoinChannel("#" + username);
                Channels.Add(new Channel(irc, "#" + username, globalCommand, permissionManager));
                irc.SendMessage("Joined channel KappaClaus");
            }
        }

        /// <summary>
        /// Leaves the given channel and removes listeners associated with it
        /// </summary>
        /// <param name="username">User whose channel to leave</param>
        /// <returns>True if bot was joined and has now left, false if it wasn't in the channel</returns>
        public bool LeaveChannel(string username)
        {
            lock(this.Channels)
            {
                for(int i = 0; i < Channels.Count; i++)
                {
                    if(username.ToLower().Equals(Channels[i].Name.ToLower()))
                    {
                        irc.SendMessage("Leaving channel... bye bye BibleThump");
                        Channels.RemoveAt(i);
                        irc.LeaveChannel("#" + username);
                        return true;
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
