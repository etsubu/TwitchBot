using System;
using System.Collections.Generic;
using System.Text;
using TwitchBot.Commands.Permissions;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Manages the permission levels for all users in a single channel
    /// PermissionCommand checks the legality of permissions updates
    /// </summary>
    internal class PermissionCommand : Command
    {

        /// <summary>
        /// PermissionCommand is not removeable
        /// </summary>
        /// <returns>False</returns>
        public override bool IsRemoveable => false;

        /// <summary>
        /// PermissionCommand is not global command
        /// </summary>
        public override bool IsGlobal => false;

        private readonly PermissionManager permissionManager;
        private readonly ChannelName Channel;

        /// <summary>
        /// Initializes PermissionsCommand
        /// <param name="handler">Ununsed by the PermissionCommand</param>
        /// <param name="permission">PermissionManager for requesting actual permissions from</param>
        /// </summary>
        public PermissionCommand(PermissionManager permissionManager, ChannelName channel) : base("permission")
        {
            this.permissionManager = permissionManager;
            this.Channel = channel;
            
        }

        /// <summary>
        /// Processes updating or querying permissions of users
        /// </summary>
        /// <param name="line">Command line</param>
        /// <param name="sender">sender name</param>
        /// <returns>Message telling the result of the command</returns>
        public override CommandResult Process(string line, string sender, string botname)
        {
            string[] parts = line.Split(" ");

            if (parts.Length < 2)
                return new CommandResult(false, "Invalid command");

            string command = parts[1];
            string name = parts[2];

            if (command.Equals("set") && parts.Length == 4)
            {
                string permissionString = parts[3];
                // Check that the permission level is an integer between 0 - MaxPermission
                if (!int.TryParse(permissionString, out int permission) || permission < 0 || permission > PermissionManager.MaxPermission)
                    return new CommandResult(false, $"Illegal permission \"{permissionString}\" should be between 0-" + PermissionManager.MaxPermission);

                // Check that the user requesting the permission update has required permission
                // person can only change others permission IF he has higher permission, and the
                // updated permission must be lower than his own
                // => you cannot change someones permission if he has the same level as you
                // and you cannot make someone the same permission level as you
                int senderPermission = permissionManager.QueryPermission(Channel, sender, botname);
                int personToBeUpdatedPermission = permissionManager.QueryPermission(Channel, name, botname);
                if (senderPermission > personToBeUpdatedPermission)
                {
                    if (permission < senderPermission)
                    {
                        permissionManager.UpdatePermission(Channel, name, permission, botname);
                        return new CommandResult(true, $"Permission for {name} set to {permission}");
                    }
                    return new CommandResult(false, $"{sender} The permission you set for others must be lower than yours");
                } else
                {
                    return new CommandResult(false, $"{sender} You lack the permission to change the permission level of {name}");
                }
            }

            if (command.Equals("query") && parts.Length == 3)
            {
                return new CommandResult(true, $"Permission for {name} is {permissionManager.QueryPermission(Channel, name, botname)}");
            }

            return new CommandResult(false, "Invalid command");
        }

        public override string Help()
        {
            return "Usage: !permission query [USERNAME] --- !permission set [NAME] [PERMISSION_LEVEL(0-" + PermissionManager.MaxPermission + ")]";
        }
    }
}
