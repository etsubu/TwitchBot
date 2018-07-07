using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Handles sending all the broadcast messages and adding, removing, modifying them
    /// </summary>
    internal class BroadcastCommand : Command
    {
        public override bool IsRemoveable => false;

        /// <summary>
        /// BroadcastCommand is not global command
        /// </summary>
        public override bool IsGlobal => false;

        public string Channel { get; }

        private readonly IRC irc;
        private readonly List<BroadcastMessage> messages;
        private readonly Timer timer;
        private int tickCount;

        /// <summary>
        /// Initializes the BroadcastCommand
        /// </summary>
        /// <param name="irc">IRC object to use for sending broadcast messages</param>
        /// <param name="channel">Channel to send the broadcast messages to</param>
        public BroadcastCommand(IRC irc, string channel):base("broadcast", 1)
        {
            this.irc = irc;
            this.Channel = channel;
            tickCount = 0;
            timer = new Timer(1000);
            messages = new List<BroadcastMessage>();
            timer.Elapsed += OnTimerTick;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        /// <summary>
        /// Called for every timer tick to check if any of the broadcast messages should be sent
        /// </summary>
        /// <param name="source">Ununsed</param>
        /// <param name="e">Ununsed</param>
        private void OnTimerTick(Object source, ElapsedEventArgs e)
        {
            lock (messages)
            {
                foreach (BroadcastMessage message in messages)
                {
                    if (message.IsTiming(tickCount))
                    {
                        irc.SendMessage(message.GetMessage(), Channel);
                    }
                }
            }

            tickCount++;
            Console.WriteLine(tickCount);
        }

        /// <summary>
        /// Tries to add new broadcast message
        /// </summary>
        /// <param name="parts">Parts of the command line</param>
        /// <returns>True if new broadcast message was added, false if failed</returns>
        private bool AddBroadcast(string[] parts)
        {
            if (!int.TryParse(parts[2], out var delay))
                return false;

            string message = string.Join(" ", parts, 3, parts.Length - 3);
            lock (messages)
            {
                messages.Add(new BroadcastMessage(delay, message));
            }

            return true;
        }

        /// <summary>
        /// Tries to remove a broadcast message
        /// </summary>
        /// <param name="parts">Parts of the command line</param>
        /// <returns>True if the broadcast message was removed, false if failed</returns>
        private bool RemoveBroadcast(string[] parts)
        {
            // If no index was given remove the latest
            if (parts.Length == 2)
            {
                lock (messages)
                {
                    if (messages.Count == 0)
                        return false;

                    lock (messages)
                    {
                        messages.RemoveAt(messages.Count - 1);
                    }
                    return true;
                }
            }

            if (!int.TryParse(parts[2], out var index) || index < 1)
                return false;

            index--;
            lock (messages)
            {
                if (index >= messages.Count)
                    return false;

                messages.RemoveAt(index);
                return true;
            }
        }

        /// <summary>
        /// Lists all the existing broadcast messages
        /// </summary>
        /// <returns>All the broadcast messages</returns>
        private string ListBroadcasts()
        {
            var broadcasts = new StringBuilder();
            lock (messages)
            {
                if (messages.Count == 0)
                    broadcasts.Append("There are no broadcast messages");
                else
                {
                    var count = 1;
                    foreach (var message in messages)
                    {
                        broadcasts.AppendLine(
                            $"{count}. ({message.GetDelay()} seconds): {message.GetMessage()} _______ ");
                        count += 1;
                    }
                }
            }

            return broadcasts.ToString();
        }

        /// <summary>
        /// Processes adding, removing and listing of broadcast messages
        /// </summary>
        /// <param name="line">Command line</param>
        /// <param name="sender">Name of the sender</param>
        /// <returns>Response to the command</returns>
        public override CommandResult Process(string line, string sender)
        {
            string[] parts = line.Split(" ");
            if (parts.Length < 2)
                return new CommandResult(false, "Invalid command");

            if (parts[1].Equals("add") && parts.Length > 3)
            {
                if (AddBroadcast(parts))
                    return new CommandResult(true, "Broadcast message was added.");

                return new CommandResult(false, "Failed to add broadcast message");
            }

            if (parts[1].Equals("remove"))
            {
                if (RemoveBroadcast(parts))
                    return new CommandResult(true, "Broadcast message was removed");
                return new CommandResult(false, "Failed to remove broadcast message");
            }

            if (parts[1].Equals("list"))
                return new CommandResult(true, ListBroadcasts());

            return new CommandResult(false, "Unknown command");
        }

        /// <summary>
        /// Implements help interface
        /// </summary>
        /// <returns>help text</returns>
        public override string Help()
        {
            return "Usage: !broadcast list -- !broadcast add [TEXT_TO_BROADCAST] [DELAY_IN_SECONDS] --- " +
                "!broadcast remove [ID_OF_BROADCASTMESSAGE]";
        }
    }
}
