using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using TwitchBot.Commands.LocalCommands;
using TwitchBot.Commands.Permissions;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Processes all the commands sent for the bot in a single channel
    /// </summary>
    internal class CommandHandler
    {
        private readonly Dictionary<string, Command> commands;
        private readonly PermissionManager permissionManager;
        private readonly IRC irc;
        public readonly ChannelName channelName;
        private Database database;

        /// <summary>
        /// Initializes CommandHandler
        /// </summary>
        /// <param name="irc">IRC object to use for sending messages</param>
        /// <param name="channelOwner">Name of the channel owner</param>
        /// <param name="permissionManager">PermissionManager object that contains all permission levels</param>
        public CommandHandler(IRC irc, ChannelName channelName, GlobalCommand globalCommand, PermissionManager permissionManager, Database database)
        {
            this.irc = irc;
            this.channelName = channelName;
            this.database = database;

            commands = database.QueryBasicCommands(channelName); // Load the initial basic commands from the database
            this.permissionManager = permissionManager;
            BroadcastCommand broadcast = new BroadcastCommand(irc, channelName);

            var services = new ServiceCollection()
                .AddSingleton(irc)
                .AddSingleton<Command, MetaCommand>()
                .AddSingleton<Command, UptimeCommand>()
                .AddSingleton<Command, PermissionCommand>()
                .AddSingleton<Command, BroadcastCommand>()
                .AddSingleton<Command, AuthorCommand>()
                .AddSingleton<Command, HelpCommand>()
                .AddSingleton(this)
                .AddSingleton(permissionManager)
                .AddSingleton(channelName);

            var provider = services.BuildServiceProvider();

            // instantiate/initialise commands by fetching them from the DI provider
            // BroadcastCommand is instantiated when its added as a singleton
            //provider.GetRequiredService<UptimeCommand>();
            //provider.GetRequiredService<MetaCommand>();

            foreach (var command in provider.GetServices<Command>())
            {
                Console.WriteLine(command.Name);
                commands.Add(command.Name, command);
            }
            // Add global command
            commands.Add(globalCommand.Name, globalCommand);
        }

        /// <summary>
        /// Lists all the names of available commands
        /// </summary>
        /// <returns>List of command names</returns>
        public string ListCommands()
        {
            var commandNames = new StringBuilder();
            lock (commands)
            {
                foreach (var key in commands.Keys)
                    commandNames.Append($"!{key} ");
            }

            return commandNames.ToString();
        }

        /// <summary>
        /// Processes the given command
        /// </summary>
        /// <param name="line">Command line to process</param>
        /// <param name="sender">Name of the user who sent the command</param>
        /// <param name="channel">channel the message was sent to</param>
        /// <returns>True if the command was processed, false if failed</returns>
        public bool ProcessCommand(string line, string sender, ChannelName channel)
        {
            if (line.Length < 2 || line[0] != '!')
                return false;
            line = line.Substring(1);
            int index = line.IndexOf(" ", StringComparison.Ordinal);

            lock (commands)
            {
                //Parse command name
                string name;
                if (index == -1)
                    name = line;
                else
                    name = line.Substring(0, index);

                if (!commands.ContainsKey(name))
                {
                    irc.SendMessage("Unknown command", channel);
                    return false;
                }

                //Check if the sender has permission to use the requested command
                bool hasPermission = false;
                hasPermission = (commands[name].IsGlobal) ? 
                    commands[name].HasPermission(permissionManager.QueryGlobalPermission(sender))
                    : commands[name].HasPermission(permissionManager.QueryPermission(channelName, sender));

                if (!hasPermission)
                {
                    irc.SendMessage($"{sender} You lack the permission to use this command", channel);
                    return false;
                }
                //Execute the command and send the response
                irc.SendMessage(commands[name].Process(line, sender).Response, channel);
            }

            return true;
        }

        /// <summary>
        /// Removes the given command
        /// </summary>
        /// <param name="key">Name of the command to remove</param>
        /// <returns>True if the command was removed, false if it did not exist</returns>
        public bool RemoveCommand(string key)
        {
            lock(commands)
            {
                if (!commands.ContainsKey(key))
                    return false;

                //Built in commands are not removeable
                if (commands[key].IsRemoveable && database.RemoveBasicCommand(channelName, key))
                    return commands.Remove(key);

                return false;
            }
        }

        /// <summary>
        /// Adds a new BasicCommand with the given parameters
        /// </summary>
        /// <param name="key">Name of the command</param>
        /// <param name="response">Static response of the command</param>
        /// <returns>True if the command was added, false if the given command already exists</returns>
        public bool AddCommand(string key, string response)
        {
            lock(commands)
            {
                BasicCommand cmd = new BasicCommand(key, response);
                // If the command exists try to update it
                if (commands.ContainsKey(key))
                {
                    // Try to update the command to database
                    if(database.UpdateBasicCommand(channelName, key, response))
                    {
                        // Overwrite the existing command
                        commands[cmd.Name] = new BasicCommand(key, response);
                        return true;
                    }
                    return false;
                }
                if (database.AddBasicCommand(channelName, key, response))
                {
                    commands[cmd.Name] = cmd;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Retrieves help text for a command if it exists
        /// </summary>
        /// <param name="commandName">Name of the command to get help text for</param>
        /// <returns>CommandResult containing the help text if the command exists</returns>
        public CommandResult GetHelp(string commandName)
        {
            // Remove prefix if it exists and force to lowercase
            commandName = (commandName.StartsWith("!") ? commandName.ToLower().Substring(1) : commandName.ToLower());
            lock (commands)
            {
                if (commands.ContainsKey(commandName))
                {
                    return new CommandResult(true, commands[commandName].Help());
                }
                return new CommandResult(false, "Unknown command name: " + commandName + " use !command list to display available commands");
            }
        }
    }
}
