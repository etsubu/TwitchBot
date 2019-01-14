using System;
using System.Collections.Generic;
using System.Text;
using TwitchBot.Commands.MessageFilters;

namespace TwitchBot.Commands.LocalCommands
{
    internal class PermitCommand : Command
    {
        public override bool IsRemoveable => false;
        public override bool IsGlobal => false;

        private readonly MessageFilterHandler filter;

        public PermitCommand(MessageFilterHandler filter) : base("permit", 2)
        {
            this.filter = filter;
        }

        public override string Help()
        {
            return "This command gives a user permission to post a single link (the permission will expire in 1 minute if not used). Usage: !permit USER";
        }

        public override CommandResult Process(string line, string sender)
        {
            string[] parts = line.Split(" ");
            if(parts.Length != 2)
            {
                return new CommandResult(false, "Invalid command. Usage: !permit USER");
            }
            return filter.AddPermit(parts[1]);
        }
    }
}
