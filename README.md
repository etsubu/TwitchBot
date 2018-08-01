# TwitchBot
This bot contains basic functionality for twitch such as custom commands with static response, broadcast messages, built-in permission system, uptime.
Also it can be active on multiple channels at the same time and configurations are channel specific. Global moderators can make the bot to join/leave channels.

## TODO ##
* Vote polls
* Permissions for posting links
* Song requests (?)
* More...

## Commands ##
Variable parameters are written here with lesser than '<' and greater than '> characters. Example: !command add <command_name>
Type variable without the <> characters e.g. !command add brackets
* !command
   * !command list
    * Displays all available commands
    * Example: !command list
   * !command add <command_name> <response>
    * Adds a new command with static response
    * Example: !command add brackets example.com    --- This creates command !brackets with response "example.com"
   * !command remove <command_name>
    * Removes a command created with !command add
    * Example: !command remove brackets   --- This removes command !brackets
* !permission
    * Permission command is used to configure permission levels for the bots built-in permissions system
    * !permission set <user> <0-3>
      * This sets users permission level to given number between [0-3]. Command issuer must have higher permission than the user whose permission he is about to change. Also the max permission he can give is his own permission level -1
        * 0 Means no permissions which is the default for viewers
        * 1 Means "Trusted". This doesn't give permission to configure the bot but persons messages e.g. links won't be monitored or removed
        * 2 Means "moderator" this is automatically given to twitch chat moderators and gives permission to configure bots commands
        * 3 Means "super moderator" same permissions as moderator but can also change normal moderators permission level
        * 4 Is the permission level of the channel owner. This permission level cannot be given to others
    * Example: !permission set nagrodusbot 1 --- This sets the user nagrodusbot to permission level 1
   * !permission query <name>
    * This displays the permission level of the given user
    * Example: !permission query nagrodusbot --- This displays the permission level of nagrodusbot
 
