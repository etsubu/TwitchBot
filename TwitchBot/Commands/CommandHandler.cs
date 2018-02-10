using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Processes all the commands sent for the bot
    /// </summary>
    class CommandHandler
    {
        private Dictionary<string, Command> commands;
        private IRC irc;

        /// <summary>
        /// Initializes CommandHandler
        /// </summary>
        /// <param name="irc">IRC object to use for sending messages</param>
        public CommandHandler(IRC irc)
        {
            this.irc = irc;
            this.commands = new Dictionary<string, Command>();
            InitCommands();
        }

        /// <summary>
        /// Initializes the existing commands
        /// </summary>
        private void InitCommands()
        {
            BasicCommand cmd = new BasicCommand("hello", "hoihoi");
            this.commands.Add(cmd.GetName(), cmd);
        }

        /// <summary>
        /// Processes the given command
        /// </summary>
        /// <param name="line">Command line to process</param>
        /// <returns>True if the command was processed, false if failed</returns>
        public bool ProcessCommand(string line)
        {
            if (line.Length < 2 || line[0] != '!')
                return false;
            line = line.Substring(1);
            int index = line.IndexOf(" ");
            Command cmd;
            lock (this.commands)
            {
                if (index == -1)
                {
                    cmd = this.commands[line];
                }
                else
                {
                    cmd = this.commands[line.Substring(0, index)];
                }
                if (cmd == null)
                    return false;
                Console.WriteLine("Processing");
                irc.SendMessage(cmd.Process(line), "#nagrodus");
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
                return this.commands.Remove(key);
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
                if (this.commands.ContainsKey(key))
                    return false;
                BasicCommand cmd = new BasicCommand(key, response);
                this.commands[cmd.GetName()] = cmd;
            }
            return true;
        }
    }
}
