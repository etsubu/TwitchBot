using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot.Commands.Permissions
{
    /// <summary>
    /// Handles all user permissions for every channel and global permissions
    /// PermissionManager does not do security checks for the permission updates. 
    /// Caller is responsible to sanity check the input.
    /// PermissionManager is synchronized
    /// </summary>
    internal class PermissionManager
    {
        private readonly Dictionary<ChannelUsernamePair, int> permissions;
        public const int MaxPermission = 10;

        /// <summary>
        /// Initializes PermissionManager
        /// </summary>
        public PermissionManager()
        {
            permissions = new Dictionary<ChannelUsernamePair, int>();
        }

        /// <summary>
        /// Queries user permission for a specific channel. Channel owner has always MaxPermission
        /// </summary>
        /// <param name="channel">Channel to look for users permission in</param>
        /// <param name="username">User whose permission to retrieve</param>
        /// <returns></returns>
        public int QueryPermission(string channel, string username)
        {
            // If the user is channel owner. Return MaxPermission
            if (channel.Equals("#" + username))
                return MaxPermission;
            var pair = new ChannelUsernamePair(channel, username, false);
            lock (permissions)
            {
                return permissions.TryGetValue(pair, out var permission) ? permission : 0;
            }
        }

        /// <summary>
        /// Queries user global permission
        /// </summary>
        /// <param name="username">User whose permission to retrieve</param>
        /// <returns></returns>
        public int QueryGlobalPermission(string username)
        {
            var pair = new ChannelUsernamePair("", username, true);
            lock (permissions)
            {
                return permissions.TryGetValue(pair, out var permission) ? permission : 0;
            }
        }

        /// <summary>
        /// Updates the permission of a user in a specific channel
        /// </summary>
        /// <param name="channel">Channel in which the permission is to be updated</param>
        /// <param name="username">User whose permission will be updated</param>
        /// <param name="permission">Permission level to set</param>
        public void UpdatePermission(string channel, string username, int permission)
        {
            var pair = new ChannelUsernamePair(channel, username, false);
            lock (permissions)
            {
                if (permissions.ContainsKey(pair))
                    permissions[pair] = permission;
                else
                {
                    permissions.Add(pair, permission);
                }
            }
        }

        /// <summary>
        /// Updates the global permission of a user
        /// </summary>
        /// <param name="username">User whose global permission is to be updated</param>
        /// <param name="permission">Global permission level to set</param>
        public void UpdatePermissionGlobal(string username, int permission)
        {
            var pair = new ChannelUsernamePair("", username, true);
            lock (permissions)
            {
                if (permissions.ContainsKey(pair))
                    permissions[pair] = permission;
                else
                    permissions.Add(pair, permission);
            }
        }
    }
}
