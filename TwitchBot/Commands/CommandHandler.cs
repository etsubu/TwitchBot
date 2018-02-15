﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Processes all the commands sent for the bot
    /// </summary>
    internal class CommandHandler
    {
        private readonly Dictionary<string, Command> commands;
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

            //Channel owner always has max permission by default
            permission.SetPermission(this.channelOwner, PermissionCommand.MaxPermission);
        }

        private void RegisterCommand<TCommand>(Action<TCommand> commandConfiguration = null) where TCommand : Command
        {
            var instance = Activator.CreateInstance(typeof(TCommand), this);
        }

        /// <summary>
        /// Initializes the existing commands
        /// </summary>
        //private void InitCommands()
        //{
        //    meta = new MetaCommand(this);
        //    uptime = new UptimeCommand();
        //    permission = new PermissionCommand();
        //    broadcast = new BroadcastCommand(irc, "#" + channelOwner);
        //    commands.Add(meta.Name, meta);
        //    commands.Add(uptime.Name, uptime);
        //    commands.Add(permission.Name, permission);
        //    commands.Add(broadcast.Name, broadcast);

        //    //Channel owner always has max permission by default
        //    permission.SetPermission(channelOwner, PermissionCommand.MaxPermission);
        //}

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
                if (!commands[name].HasPermission(permission.QueryPermission(sender)))
                {
                    irc.SendMessage($"{sender} You lack the permission to use this command", channel);
                    return false;
                }
                CommandResult result = commands[name].Process(line, sender);
                if (result.Succeeded)
                {
                    // TODO
                }
                else
                {
                    // TODO
                }
                irc.SendMessage(result.Response, channel);
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
