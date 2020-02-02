using System;
using System.Collections.Generic;
using System.Text;
using TwitchBot.Commands.MessageFilters;

namespace TwitchBot.Commands.LocalCommands
{
    internal class UnicodeCommand : Command
    {
        public override bool IsRemoveable => false;
        public override bool IsGlobal => false;

        private MessageFilterHandler filter;

        public UnicodeCommand(MessageFilterHandler filter) : base("unicode")
        {
            this.filter = filter;
        }

        public override string Help()
        {
            return "Usage !unicode true    !unicode false     To allow or disallow unicode messages from nontrusted users";
        }

        public override CommandResult Process(string line, string sender, string botname)
        {
            line = line.ToLower();
            string[] parts = line.Split(" ");
            if (parts.Length == 1)
            {
                return new CommandResult(true, (filter.LinksAllowed) ? "Unicode pasta is allowed!" : "Unicode pasta is not allowed!");
            }
            if (parts[1].Equals("1") || parts[1].Equals("true") || parts[1].Equals("allow"))
            {
                filter.UnicodeAllowed = true;
                return new CommandResult(true, "unicode allowed!");
            }
            else if (parts[1].Equals("0") || parts[1].Equals("false") || parts[1].Equals("disallow"))
            { 
                filter.UnicodeAllowed = false;
                return new CommandResult(true, "unicode from nontrusted users have been blocked!");
            }
            return new CommandResult(false, $"@{sender} Unknown boolean value \"{line}\" try true/false or 0/1");
        }
    }
}
