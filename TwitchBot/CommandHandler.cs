using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot
{
    /// <summary>
    /// Processes all the commands sent for the bot
    /// </summary>
    class CommandHandler
    {
        private Dictionary<string, Command> commands;
        private IRC irc;
        private MetaCommand meta;
        private UptimeCommand uptime;
        private PermissionCommand permission;
        private string channelOwner;

        /// <summary>
        /// Initializes CommandHandler
        /// </summary>
        /// <param name="irc">IRC object to use for sending messages</param>
        /// <param name="channelOwner">Name of the channel owner</param>
        public CommandHandler(IRC irc, string channelOwner)
        {
            this.irc = irc;
            this.channelOwner = channelOwner;
            this.commands = new Dictionary<string, Command>();
            InitCommands();
        }

        /// <summary>
        /// Initializes the existing commands
        /// </summary>
        private void InitCommands()
        {
            this.meta = new MetaCommand(this);
            this.uptime = new UptimeCommand();
            this.permission = new PermissionCommand();
            this.commands.Add(this.meta.GetName(), this.meta);
            this.commands.Add(this.uptime.GetName(), this.uptime);
            this.commands.Add(this.permission.GetName(), this.permission);

            //Channel owner always has max permission by default
            this.permission.SetPermission(this.channelOwner, PermissionCommand.MAX_PERMISSION);
        }

        /// <summary>
        /// Lists all the names of available commands
        /// </summary>
        /// <returns>List of command names</returns>
        public string ListCommands()
        {
            string commandNames = "";
            lock(this.commands)
            {
                foreach(string key in this.commands.Keys) {
                    commandNames += "!" + key + " ";
                }
            }
            return commandNames;
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
            int index = line.IndexOf(" ");
            lock (this.commands)
            {
                //Parse command name
                string name;
                if (index == -1)
                {
                    name = line;
                }
                else
                {
                    name = line.Substring(0, index);
                }
                if (!this.commands.ContainsKey(name))
                {
                    irc.SendMessage("Unknown command", channel);
                    return false;
                }
                //Check if the sender has permission to use the requested command
                if(!this.commands[name].HasPermission(this.permission.QueryPermission(sender)))
                {
                    irc.SendMessage(sender + " You lack the permission to use this command", channel);
                    return false;
                }
                //Execute the command and send the response
                irc.SendMessage(this.commands[name].Process(line), channel);
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
            lock(this.commands)
            {
                if (!this.commands.ContainsKey(key))
                    return false;
                //Built in commands are not removeable
                if(this.commands[key].IsRemoveable())
                    return this.commands.Remove(key);
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
            lock(this.commands)
            {
                //Only add the command if it doesn't already exist
                if (this.commands.ContainsKey(key))
                    return false;
                BasicCommand cmd = new BasicCommand(key, response);
                this.commands[cmd.GetName()] = cmd;
            }
            return true;
        }
    }
}
