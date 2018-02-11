using System;
using System.Collections.Generic;
using System.Timers;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Handles sending all the broadcast messages and adding, removing, modifying them
    /// </summary>
    internal class BroadcastCommand : Command
    {
        public override bool IsRemoveable => false;

        private IRC irc;
        private List<BroadcastMessage> messages;
        private Timer timer;
        private int tickCount;
        private string channel;

        /// <summary>
        /// Initializes the BroadcastCommand
        /// </summary>
        /// <param name="irc">IRC object to use for sending broadcast messages</param>
        /// <param name="channel">Channel to send the broadcast messages to</param>
        public BroadcastCommand(IRC irc, string channel):base("broadcast", 1)
        {
            this.irc = irc;
            this.channel = channel;
            this.tickCount = 0;
            this.timer = new Timer(1000);
            this.messages = new List<BroadcastMessage>();
            this.timer.Elapsed += OnTimerTick;
            this.timer.AutoReset = true;
            this.timer.Enabled = true;
        }

        /// <summary>
        /// Called for every timer tick to check if any of the broadcast messages should be sent
        /// </summary>
        /// <param name="source">Ununsed</param>
        /// <param name="e">Ununsed</param>
        private void OnTimerTick(Object source, ElapsedEventArgs e)
        {
            lock (this.messages)
            {
                foreach (BroadcastMessage message in this.messages)
                {
                    if(message.IsTiming(this.tickCount))
                    {
                        this.irc.SendMessage(message.GetMessage(), this.channel);
                    }
                }
            }
            this.tickCount++;
            Console.WriteLine(this.tickCount);
        }

        /// <summary>
        /// Tries to add new broadcast message
        /// </summary>
        /// <param name="parts">Parts of the command line</param>
        /// <returns>True if new broadcast message was added, false if failed</returns>
        private bool AddBroadcast(string[] parts)
        {
            if (!int.TryParse(parts[2], out var delay))
            {
                return false;
            }
            string message = string.Join(" ", parts, 3, parts.Length - 3);
            lock (this.messages)
            {
                this.messages.Add(new BroadcastMessage(delay, message));
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
                lock (this.messages)
                {
                    if (this.messages.Count == 0)
                    {
                        return false;
                    }
                    lock (this.messages)
                    {
                        this.messages.RemoveAt(this.messages.Count - 1);
                    }
                    return true;
                }
            }
            else
            {
                if (!int.TryParse(parts[2], out var index) || index < 1)
                {
                    return false;
                }
                index--;
                lock (this.messages)
                {
                    if (index >= this.messages.Count)
                    {
                        return false;
                    }
                    this.messages.RemoveAt(index);
                    return true;
                }
            }
        }

        /// <summary>
        /// Lists all the existing broadcast messages
        /// </summary>
        /// <returns>All the broadcast messages</returns>
        private string ListBroadcasts()
        {
            string broadcasts = "";
            lock (this.messages)
            {
                if (this.messages.Count == 0)
                    return "There are no broadcast messages";
                for (int i = 0; i < this.messages.Count; i++)
                {
                    broadcasts += (i + 1) + ". (" + this.messages[i].GetDelay() + " seconds): " + this.messages[i].GetMessage() + " _______ ";
                }
            }
            return broadcasts;
        }

        /// <summary>
        /// Processes adding, removing and listing of broadcast messages
        /// </summary>
        /// <param name="line">Command line</param>
        /// <param name="sender">Name of the sender</param>
        /// <returns>Response to the command</returns>
        public override string Process(string line, string sender)
        {
            string[] parts = line.Split(" ");
            if (parts.Length < 2)
                return "Invalid command";
            if(parts[1].Equals("add") && parts.Length > 3)
            {
                if (AddBroadcast(parts))
                    return "Broadcast message was added.";
                else
                    return "Failed to add broadcast message";
            } else if(parts[1].Equals("remove"))
            {
                if (RemoveBroadcast(parts))
                    return "Broadcast message was removed";
                else
                    return "Failed to remove broadcast message";
            } else if(parts[1].Equals("list"))
            {
                return ListBroadcasts();
            }
            return "Unknown command";
        }
    }
}
