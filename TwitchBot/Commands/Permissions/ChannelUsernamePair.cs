using System;

namespace TwitchBot.Commands.Permissions
{
    internal struct ChannelUsernamePair
    {
        // the struct is completely immutable
        public readonly string Channel;
        public readonly string Username;
        public readonly bool IsGlobal;

        /// <summary>
        /// Initializes ChannelUsernamePair
        /// </summary>
        /// <param name="channel">Channel name this user is for</param>
        /// <param name="username">Username</param>
        /// <param name="isGlobal">True if the channel should be ignored</param>
        public ChannelUsernamePair(string channel, string username, bool isGlobal)
        {
            Channel = channel;
            Username = username;
            IsGlobal = isGlobal;
        }

        // two ChannelUsernamePairs are considered equal if their Channel and Username match
        public bool Equals(ChannelUsernamePair other) => 
            string.Equals(Channel, other.Channel) && string.Equals(Username, other.Username) && IsGlobal == other.IsGlobal;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            return obj is ChannelUsernamePair container && Equals(container);
        }

        public override int GetHashCode()
        {
            return Channel.GetHashCode() ^ Username.GetHashCode() ^ IsGlobal.GetHashCode();
        }
    }
}
