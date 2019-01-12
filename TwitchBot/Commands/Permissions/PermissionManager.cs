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
        public const int MaxPermission = 3;
        private Database database;
        private string owner;

        /// <summary>
        /// Initializes PermissionManager
        /// <param name="database">Database object to use for synchronizing and querying permission levels</param>
        /// <param name="owner">Owner of this bot will have global permission of level MaxPermission</param>
        /// </summary>
        public PermissionManager(Database database, string owner)
        {
            this.owner = owner;
            this.database = database;
            permissions = database.QueryPermissions();
            // Set bot owners global permission to MaxPermission
            permissions.Add(new ChannelUsernamePair(new ChannelName(), owner, true), MaxPermission);
        }

        /// <summary>
        /// Queries user permission for a specific channel. Channel owner has always MaxPermission
        /// </summary>
        /// <param name="channel">Channel to look for users permission in</param>
        /// <param name="username">User whose permission to retrieve</param>
        /// <returns></returns>
        public int QueryPermission(ChannelName channel, string username)
        {
            // If the user is channel owner OR bot owner. Return MaxPermission
            if (channel.Equals(new ChannelName(username)) || username.Equals(owner))
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
            var pair = new ChannelUsernamePair(new ChannelName(), username, true);
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
        public void UpdatePermission(ChannelName channel, string username, int permission)
        {
            var pair = new ChannelUsernamePair(channel, username, false);
            lock (permissions)
            {
                if (permissions.ContainsKey(pair))
                {
                    if (permission == 0)
                    {
                        permissions.Remove(pair);
                        database.DeletePermission(pair);
                    }
                    else
                    {
                        permissions[pair] = permission;
                        database.UpdatePermission(pair, permission);
                    }
                }
                else if (permission > 0)
                {
                    permissions.Add(pair, permission);
                    database.AddPermission(pair, permission);
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
            var pair = new ChannelUsernamePair(new ChannelName(), username, true);
            lock (permissions)
            {
                if (permissions.ContainsKey(pair))
                {
                    if (permission == 0)
                    {
                        permissions.Remove(pair);
                        database.DeletePermission(pair);
                    }
                    else
                    {
                        permissions[pair] = permission;
                        database.UpdatePermission(pair, permission);
                    }
                }
                else if (permission > 0)
                {
                    permissions.Add(pair, permission);
                    database.AddPermission(pair, permission);
                }
            }
        }
    }
}
