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
            return "Usage !unicode true    !unicode false     To allow or disallow unicode messages from non trusted users";
        }

        public override CommandResult Process(string line, string sender)
        {
            line = line.ToLower();
            if (line.Equals("1") || line.Equals("true") || line.Equals("allow"))
            {
                filter.UnicodeAllowed = true;
                return new CommandResult(true, "unicode allowed!");
            }
            else if (line.Equals("0") || line.Equals("false") || line.Equals("disallow"))
            {
                filter.UnicodeAllowed = false;
                return new CommandResult(true, "unicode from non trusted users have been blocked!");
            }
            return new CommandResult(false, $"@{sender} Unknown boolean value \"{line}\" try true/false or 0/1");
        }
    }
}
