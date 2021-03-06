﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot.Commands
{
    /// <summary>
    /// This acts as a base command for other global commands. All global commands are called ith !global ...
    /// </summary>
    internal class GlobalCommand : Command
    {

        public override bool IsRemoveable => false;
        public override bool IsGlobal => true;

        public ChatBot Bot;

        public GlobalCommand(ChatBot bot) : base ("global")
        {
            Bot = bot;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Help text, duh</returns>
        public override string Help()
        {
            return "If you do not know the global commands. You shouldn't be messing with them";
        }

        public override CommandResult Process(string line, string sender, string botname)
        {
            return new CommandResult(false, "Unimplemented!");
        }

        public CommandResult Process(string line, string sender, IRC irc)
        {
            // TODO clean this up later
            string[] parts = line.Split(" ");
            if (parts.Length > 2 && parts[1].Equals("leave"))
            {
                Bot.LeaveChannel(new ChannelName(parts[2]));
                return new CommandResult(true, "Left channel " + parts[2]);
            }
            else if (parts.Length > 2 && parts[1].Equals("broadcast"))
            {
                Bot.BroadcastToChannels(string.Join(" ", parts, 2, parts.Length - 2));
                return new CommandResult(true, "Broadcasted message to all channels");
            }
            else if (parts.Length == 3 && parts[1].Equals("join"))
            {
                if (Bot.JoinToChannel(parts[2], irc))
                    return new CommandResult(true, "Joined to channel " + parts[2]);
                else
                    return new CommandResult(false, "Failed to join channel " + parts[2] + " Maybe one instance is already on the channel?");
            }
            return new CommandResult(false, "Invalid command");
        }
    }
}
