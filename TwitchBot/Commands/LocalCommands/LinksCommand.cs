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
            return "Usage !links true    !link false     To allow or disallow links from none trusted users";
        }

        public override CommandResult Process(string line, string sender)
        {
            line = line.ToLower();
            if(line.Equals("1") || line.Equals("true") || line.Equals("allow"))
            {
                filter.LinksAllowed = true;
                return new CommandResult(true, "links allowed!");
            }
            else if(line.Equals("0") || line.Equals("false") || line.Equals("disallow"))
            {
                filter.LinksAllowed = false;
                return new CommandResult(true, "links from none trusted users have been blocked!");
            }
            return new CommandResult(false, $"@{sender} Unknown boolean value \"{line}\" try true/false or 0/1");
        }
    }
}
