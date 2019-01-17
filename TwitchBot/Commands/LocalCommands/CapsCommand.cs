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
            return "Usage !caps true    !caps false     To allow or disallow only caps messages from non trusted users";
        }

        public override CommandResult Process(string line, string sender)
        {
            line = line.ToLower();
            if (line.Equals("1") || line.Equals("true") || line.Equals("allow"))
            {
                filter.CapsAllowed = true;
                return new CommandResult(true, "caps allowed!");
            }
            else if (line.Equals("0") || line.Equals("false") || line.Equals("disallow"))
            {
                filter.CapsAllowed = false;
                return new CommandResult(true, "caps from non trusted users have been blocked!");
            }
            return new CommandResult(false, $"@{sender} Unknown boolean value \"{line}\" try true/false or 0/1");
        }
    }
}
