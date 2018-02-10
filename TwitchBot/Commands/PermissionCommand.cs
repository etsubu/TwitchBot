using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Manages the permission levels for all users
    /// </summary>
    internal class PermissionCommand : Command
    {
        public const int MaxPermission = 10;
        private readonly Dictionary<string, int> permissions;

        /// <summary>
        /// Initializes PermissionsCommand
        /// </summary>
        public PermissionCommand() : base("permission")
        {
            permissions = new Dictionary<string, int>();
        }

        /// <summary>
        /// PermissionCommand is not removeable
        /// </summary>
        /// <returns>False</returns>
        public override bool IsRemoveable()
        {
            return false;
        }

        /// <summary>
        /// Queries the permission level for the given user. Undefined users have permission level of 0
        /// </summary>
        /// <param name="name">User to query</param>
        /// <returns>Permission level of 0 to 10</returns>
        public int QueryPermission(string name)
        {
            lock(permissions)
            {
                if (permissions.ContainsKey(name))
                    return permissions[name];

                return 0;
            }
        }

        /// <summary>
        /// Sets the given permission level for a user
        /// </summary>
        /// <param name="name">Name of the user to set permission level for</param>
        /// <param name="permission">Permission level to set</param>
        public void SetPermission(string name, int permission)
        {
            lock(permissions)
            {
                permissions[name] = permission;
            }
        }

        /// <summary>
        /// Processes updating or querying permissions of users
        /// </summary>
        /// <param name="line">Command line</param>
        /// <returns>Message telling the result of the command</returns>
        public override string Process(string line)
        {
            string[] parts = line.Split(" ");

            if (parts.Length < 2)
                return "Invalid command";

            if (parts[1].Equals("set") && parts.Length == 4)
            {
                if (!int.TryParse(parts[3], out int permission) || permission < 0 || permission > MaxPermission)
                    return "Illegal permission \"" + parts[3] + "\"";

                SetPermission(parts[2], permission);
                return "Permission for " + parts[2] + " set to " + permission;
            }

            if (parts[1].Equals("query") && parts.Length == 3)
                return "Permission for " + parts[2] + " is " + QueryPermission(parts[2]);

            return "Invalid command";
        }
    }
}
