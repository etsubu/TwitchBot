using System;
using System.Collections.Generic;
using System.Text;
using TwitchBot.Commands.MessageFilters;

namespace TwitchBot.Commands.LocalCommands
{
    internal class CapsCommand : Command
    {
        public override bool IsRemoveable => false;
        public override bool IsGlobal => false;

        private MessageFilterHandler filter;

        public CapsCommand(MessageFilterHandler filter) : base("caps")
        {
            this.filter = filter;
        }

        public override string Help()
        {
            return "Usage !caps true    !caps false     To allow or disallow only caps messages from nontrusted users";
        }

        public override CommandResult Process(string line, string sender)
        {
            line = line.ToLower();
            string[] parts = line.Split(" ");
            if (parts.Length == 1)
            {
                return new CommandResult(true, (filter.CapsAllowed) ? "Caps spam is allowed!" : "Caps spam is not allowed!");
            }
            if (parts[1].Equals("1") || parts[1].Equals("true") || parts[1].Equals("allow"))
            {
                filter.CapsAllowed = true;
                return new CommandResult(true, "caps allowed!");
            }
            else if (parts[1].Equals("0") || parts[1].Equals("false") || parts[1].Equals("disallow"))
            {
                filter.CapsAllowed = false;
                return new CommandResult(true, "caps from nontrusted users have been blocked!");
            }
            return new CommandResult(false, $"@{sender} Unknown boolean value \"{line}\" try true/false or 0/1");
        }
    }
}
