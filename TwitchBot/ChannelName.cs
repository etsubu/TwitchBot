using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot
{
    /// <summary>
    /// Tiny wrapper for IRC channel name to make sure '#' is used properly and that channel name is lowercase
    /// </summary>
    internal class ChannelName
    {
        private string channelName;
        
        public ChannelName(string channelName)
        {
            this.channelName = (channelName.StartsWith("#") ? channelName.ToLower() : ("#" + channelName.ToLower()));
        }

        public ChannelName()
        {
            this.channelName = "##";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>The owner of the channel</returns>
        public string GetOwner()
        {
            return channelName.Substring(1);
        }

        public bool Equals(ChannelName other) =>
            string.Equals(channelName, other.channelName);

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            return obj is ChannelName container && Equals(container);
        }

        public override int GetHashCode()
        {
            return channelName.GetHashCode();
        }

        public override string ToString()
        {
            return channelName;
        }
    }
}
