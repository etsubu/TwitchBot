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
    
