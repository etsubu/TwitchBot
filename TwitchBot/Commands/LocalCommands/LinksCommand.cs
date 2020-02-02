using System;
using System.Collections.Generic;
using System.Text;
using TwitchBot.Commands.MessageFilters;

namespace TwitchBot.Commands.LocalCommands
{
    internal class LinksCommand : Command
    {
        public override bool IsRemoveable => false;
        public override bool IsGlobal => false;

        private MessageFilterHandler filter;

        public LinksCommand(MessageFilterHandler filter) : base("links")
        {
            this.filter = filter;
        }

        public override string Help()
        {
            return "Usage !links true    !link false     To allow or disallow links from nontrusted users";
        }

        public override CommandResult Process(string line, string sender, string botname)
        {
            line = line.ToLower();
            string[] parts = line.Split(" ");
            if (parts.Length == 1)
            {
                return new CommandResult(true, (filter.LinksAllowed) ? "Link are allowed!" : "Links are not allowed!");
            }
            if (parts[1].Equals("1") || parts[1].Equals("true") || parts[1].Equals("allow"))
            {
                filter.LinksAllowed = true;
                return new CommandResult(true, "links allowed!");
            }
            else if(parts[1].Equals("0") || parts[1].Equals("false") || parts[1].Equals("disallow"))
            {
                filter.LinksAllowed = false;
                return new CommandResult(true, "links from nontrusted users have been blocked!");
            }
            return new CommandResult(false, $"@{sender} Unknown boolean value \"{line}\" try true/false or 0/1");
        }
    }
}
