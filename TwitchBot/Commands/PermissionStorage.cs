using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Contains permissions for all the users. Undefined users have permission level of 0
    /// </summary>
    internal class PermissionStorage
    {
        public const int MaxPermission = 10;
        private readonly Dictionary<string, int> permissions;

        /// <summary>
        /// Initializes PermissionStorage
        /// </summary>
        public PermissionStorage()
        {
            this.permissions = new Dictionary<string, int>();
        }

        /// <summary>
        /// Gets the permission of the queried user. If the user is unknown the permission is 0
        /// </summary>
        /// <param name="name">Name of the user to query permission for</param>
        /// <returns>Permission level of the user</returns>
        public int GetPermission(string name)
        {
            lock(this.permissions)
            {
                if (!this.permissions.ContainsKey(name))
                    return 0;
                return this.permissions[name];
            }
        }

        /// <summary>
        /// Sets the permission level for a user. Checks if the user who wishes to change other users permission has the required permission himself
        /// </summary>
        /// <param name="permissionToSetFor">User to set a permission level for</param>
        /// <param name="permissionToSet">Permission level to set</param>
        /// <param name="commander">User who issued the command</param>
        /// <returns>True if the permission level was set. False if the permission level was invalid or the commander lacks permission</returns>
        public bool SetPermission(string permissionToSetFor, int permissionToSet, string commander)
        {
            if (permissionToSet < 0 || permissionToSet > MaxPermission)
                return false;
            int originalPermission = GetPermission(permissionToSetFor);
            int commanderPermission = GetPermission(commander);
            if(originalPermission >= commanderPermission || permissionToSet >= commanderPermission)
            {
                return false;
            }
            lock(this.permissions)
            {
                this.permissions[permissionToSetFor] = permissionToSet;
            }
            return true;
        }

        /// <summary>
        /// Sets the permission level for user without checking the command issuers permission. Used only for built in operations not for user commands
        /// </summary>
        /// <param name="permissionToSetFor">User to set permission for</param>
        /// <param name="permissionToSet">Permission level to set</param>
        /// <returns>True if the permission level was set, false if the permission level was invalid</returns>
        public bool SetHardPermission(string permissionToSetFor, int permissionToSet)
        {
            if (permissionToSet < 0 || permissionToSet > MaxPermission)
                return false;
            lock (this.permissions)
            {
                this.permissions[permissionToSetFor] = permissionToSet;
            }
            return true;
        }
    }
}
