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
        /// <summary>
        /// PermissionCommand is not removeable
        /// </summary>
        /// <returns>False</returns>
        public override bool IsRemoveable => false;
        private PermissionStorage permission;

        /// <summary>
        /// Initializes PermissionsCommand
        /// <param name="handler">Ununsed by the PermissionCommand</param>
        /// <param name="permission">PermissionStorage for handling the permissions</param>
        /// </summary>
        public PermissionCommand(CommandHandler handler, PermissionStorage permission) : base(handler, "permission")
        {
            this.permission = permission;
        }

        /// <summary>
        /// Queries the permission level for the given user. Undefined users have permission level of 0
        /// </summary>
        /// <param name="name">User to query</param>
        /// <returns>Permission level of 0 to 10</returns>
        public int QueryPermission(string name)
        {
            return permission.GetPermission(name);
        }

        /// <summary>
        /// Sets the given permission level for a user
        /// </summary>
        /// <param name="name">Name of the user to set permission level for</param>
        /// <param name="permissionLevel">Permission level to set</param>
        /// <param name="commandIssuer">User who issued the command</param>
        public bool SetPermission(string name, int permissionLevel, string commandIssuer)
        {
            return permission.SetPermission(name, permissionLevel, commandIssuer);
        }

        /// <summary>
        /// Processes updating or querying permissions of users
        /// </summary>
        /// <param name="line">Command line</param>
        /// <param name="sender">sender name</param>
        /// <returns>Message telling the result of the command</returns>
        public override CommandResult Process(string line, string sender)
        {
            string[] parts = line.Split(" ");

            if (parts.Length < 2)
                return new CommandResult(false, "Invalid command");

            if (parts[1].Equals("set") && parts.Length == 4)
            {
                if (!int.TryParse(parts[3], out int permission) || !SetPermission(parts[2], permission, parts[3]))
                    return new CommandResult(false, $"Illegal permission \"{parts[3]}\"");

                return new CommandResult(true, $"Permission for {parts[2]} set to {permission}");
            }

            if (parts[1].Equals("query") && parts.Length == 3)
                return new CommandResult(true, $"Permission for {parts[2]} is {QueryPermission(parts[2])}");

            return new CommandResult(false, "Invalid command");
        }
    }
}
