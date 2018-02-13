using System;

namespace TwitchBot.Commands.Permissions
{
    internal struct ChannelUsernamePair : IEquatable<ChannelUsernamePair>
    {
        // the struct is completely immutable
        public readonly string Channel;
        public readonly string Username;

        public ChannelUsernamePair(string channel, string username)
        {
            Channel = channel;
            Username = username;
        }

        // two ChannelUsernamePairs are considered equal if their Channel and Username match
        public bool Equals(ChannelUsernamePair other) =>
            string.Equals(Channel, other.Channel) && string.Equals(Username, other.Username);

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            return obj is ChannelUsernamePair container && Equals(container);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Channel?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (Username?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
