﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using TwitchBot.Commands.Permissions;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Processes all the commands sent for the bot
    /// </summary>
    internal class CommandHandler
    {
        private readonly Dictionary<string, Command> commands;
        private readonly PermissionManager permissionManager;
        private readonly IRC irc;
        private readonly string channelOwner;

        /// <summary>
        /// Initializes CommandHandler
        /// </summary>
        /// <param name="irc">IRC object to use for sending messages</param>
        /// <param name="channelOwner">Name of the channel owner</param>
        public CommandHandler(IRC irc, string channelOwner)
        {
            this.irc = irc;
            this.channelOwner = channelOwner;

            commands = new Dictionary<string, Command>();
            permissionManager = new PermissionManager();

            var services = new ServiceCollection()
                .AddSingleton(irc)
                .AddSingleton(irc)
                .AddSingleton<MetaCommand>()
                .AddSingleton<UptimeCommand>()
                .AddSingleton<PermissionCommand>()
                .AddSingleton(srv => new BroadcastCommand(
                    srv.GetRequiredService<IRC>(),
                    "#" + channelOwner,
                    this));

            var provider = services.BuildServiceProvider();

            // instantiate/initialise commands by fetching them from the DI provider
            // BroadcastCommand is instantiated when its added as a singleton
            provider.GetRequiredService<MetaCommand>();
            provider.GetRequiredService<UptimeCommand>();
            
            // TODO: global permissions for PermissionManager
            //var permission = provider.GetRequiredService<PermissionCommand>();
            //permission.SetPermission(channelOwner, PermissionCommand.MaxPermission);

            foreach (var command in provider.GetServices<Command>())
                commands.Add(command.Name, command);
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
        public bool ProcessCommand(string line, string sender, string channel)
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
                if (!commands[name].HasPermission(permissionManager.QueryPermission(channel, sender)))
                {
                    irc.SendMessage($"{sender} You lack the permission to use this command", channel);
                    return false;
                }
                //Execute the command and send the response
                irc.SendMessage(commands[name].Process(line, channel, sender).Response, channel);
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
                if (commands[key].IsRemoveable)
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
                //Only add the command if it doesn't already exist
                if (commands.ContainsKey(key))
                    return false;

                BasicCommand cmd = new BasicCommand(key, response, this);
                commands[cmd.Name] = cmd;
            }

            return true;
        }
    }
}
