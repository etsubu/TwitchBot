using System;
using System.Collections.Generic;
using System.Text;
using TwitchBot.Commands.Permissions;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Manages the permission levels for all users
    /// </summary>
    internal class PermissionCommand : Command
    {
        public const int MaxPermission = 10;

        /// <summary>
        /// PermissionCommand is not removeable
        /// </summary>
        /// <returns>False</returns>
        public override bool IsRemoveable => false;

        private readonly PermissionManager permissionManager;

        /// <summary>
        /// Initializes PermissionsCommand
        /// <param name="handler">Ununsed by the PermissionCommand</param>
        /// <param name="permission">PermissionStorage for handling the permissions</param>
        /// </summary>
        public PermissionCommand(PermissionManager permissionManager, CommandHandler handler) : base(handler, "permission")
        {
            this.permissionManager = permissionManager;
        }

        /// <summary>
        /// Processes updating or querying permissions of users
        /// </summary>
        /// <param name="line">Command line</param>
        /// <param name="sender">sender name</param>
        /// <returns>Message telling the result of the command</returns>
        public override CommandResult Process(string line, string channel, string sender)
        {
            string[] parts = line.Split(" ");

            if (parts.Length < 2)
                return new CommandResult(false, "Invalid command");

            var command = parts[1];
            var name = parts[2];
            var permissionString = parts[3];

            if (command.Equals("set") && parts.Length == 4)
            {
                if (!int.TryParse(permissionString, out int permission) || permission < 0 || permission > MaxPermission)
                    return new CommandResult(false, $"Illegal permission \"{permissionString}\"");

                permissionManager.UpdatePermission(channel, name, permission);
                return new CommandResult(true, $"Permission for {name} set to {permission}");
            }

            if (command.Equals("query") && parts.Length == 3)
                return new CommandResult(true, $"Permission for {name} is {permissionManager.QueryPermission(channel, name)}");

            return new CommandResult(false, "Invalid command");
        }

        public override string Help()
        {
            return "";
        }
    }
}
