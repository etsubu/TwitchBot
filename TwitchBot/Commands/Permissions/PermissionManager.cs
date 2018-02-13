using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot.Commands.Permissions
{
    internal class PermissionManager
    {
        private readonly Dictionary<ChannelUsernamePair, int> permissions;

        public PermissionManager()
        {
            permissions = new Dictionary<ChannelUsernamePair, int>();
        }

        public int QueryPermission(string channel, string username)
        {
            var pair = new ChannelUsernamePair(channel, username);
            return permissions.TryGetValue(pair, out var permission) ? permission : 0;
        }

        public void UpdatePermission(string channel, string username, int permission)
        {
            var pair = new ChannelUsernamePair(channel, username);
            if (permissions.ContainsKey(pair))
                permissions[pair] = permission;
            else
                permissions.Add(pair, permission);
        }
    }
}
