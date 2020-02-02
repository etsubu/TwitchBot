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
        }

        /// <summary>
        /// Queries user permission for a specific channel. Channel owner has always MaxPermission
        /// </summary>
        /// <param name="channel">Channel to look for users permission in</param>
        /// <param name="username">User whose permission to retrieve</param>
        /// <returns></returns>
        public int QueryPermission(ChannelName channel, string username, string botname)
        {
            // If the user is channel owner OR bot owner. Return MaxPermission
            if (channel.Equals(new ChannelName(username)) || username.Equals(owner))
                return MaxPermission;
            return database.QueryPermission(channel.ToString(), username, botname);
        }

        /// <summary>
        /// Queries user global permission
        /// </summary>
        /// <param name="username">User whose permission to retrieve</param>
        /// <returns></returns>
        public int QueryGlobalPermission(string username)
        {
            return database.QueryPermission(null, username, null);
        }

        /// <summary>
        /// Updates the permission of a user in a specific channel
        /// </summary>
        /// <param name="channel">Channel in which the permission is to be updated</param>
        /// <param name="username">User whose permission will be updated</param>
        /// <param name="permission">Permission level to set</param>
        public void UpdatePermission(ChannelName channel, string username, int permission, string botname)
        {
            var pair = new ChannelUsernamePair(channel, username, false);
            if(permission == 0)
            {
                database.DeletePermission(pair, botname);
            }
            if (database.QueryPermission(channel.ToString(), username, botname) != 0)
            {
                database.UpdatePermission(pair, permission, botname);
            }
            else
            {
                database.AddPermission(pair, permission, botname);
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
            if (QueryGlobalPermission(username) != 0)
            {
                if (permission == 0)
                {
                    database.DeletePermission(pair, null);
                }
                else
                {
                    database.UpdatePermission(pair, permission, null);
                }
            }
            else
            {
                database.AddPermission(pair, permission, null);
            }
        }
    }
}
