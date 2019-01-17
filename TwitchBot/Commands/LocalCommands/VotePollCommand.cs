using System;
using System.Collections.Generic;
using System.Text;
using TwitchBot.Commands.Permissions;

namespace TwitchBot.Commands.LocalCommands
{
    internal class VotePollCommand : Command
    {
        public override bool IsRemoveable => false;
        public override bool IsGlobal => false;

        private bool voteActive;
        private Dictionary<string, int> votes;
        private string voteName;
        private HashSet<string> voters;
        private PermissionManager permissionManager;
        private ChannelName channel;

        public VotePollCommand(PermissionManager permissionManager, ChannelName channel) : base("poll")
        {
            this.permissionManager = permissionManager;
            this.channel = channel;
            this.voters = new HashSet<string>();
            this.voteActive = false;
            this.voteName = "";
            this.votes = new Dictionary<string, int>();
        }

        public override string Help()
        {
            return "Usage: !poll new title | option1 | option2       View results: !poll results";
        }

        private CommandResult StartPoll(string[] parts)
        {
            if(voteActive)
            {
                return new CommandResult(false, "Previous votepoll is still active. Close it with !poll results to start a new one");
            }
            if (parts.Length < 2)
                return new CommandResult(false, "Usage !poll new title | option1 | option2");
            this.voteName = parts[0];
            this.voteActive = true;
            this.voters.Clear();
            this.votes.Clear();
            for(int i = 1; i < parts.Length; i++)
            {
                votes.Add(parts[i].Trim().ToLower(), 0);
            }
            return PollStatus();
        }

        private CommandResult PollStatus()
        {
            if(!voteActive)
            {
                return new CommandResult(false, "No votepoll is currently active!");
            }
            string status = $"Votepoll is active \"{voteName}\" vote with ";
            foreach(string option in votes.Keys)
            {
                status += $" !poll {option}";
            }
            return new CommandResult(true, status);
        }

        private CommandResult ClosePoll()
        {
            this.voteActive = false;
            string result = voteName;
            foreach(string option in votes.Keys)
            {
                result += ", " + option + ": " + votes[option] + " votes";
            }
            return new CommandResult(true, result);
        }

        private CommandResult Vote(string vote, string sender)
        {
            if(!voteActive)
            {
                return new CommandResult(false, null);
            }
            lock (voters) {
                if (voters.Contains(sender.ToLower()))
                {
                    return new CommandResult(false, null);
                }
                lock(votes)
                {
                    if(votes.ContainsKey(vote.Trim().ToLower())) {
                        votes[vote.Trim().ToLower()]++;
                        voters.Add(sender.ToLower());
                        return new CommandResult(true, null);
                    }
                }
            }
            return new CommandResult(false, $"{sender} invalid vote option!");
        }

        public override CommandResult Process(string line, string sender)
        {
            if(line.Equals("poll"))
            {
                return PollStatus();
            }
            string[] parts = line.Split(" ");
            if (parts.Length > 1 && parts[1].Equals("results"))
            {
                if(permissionManager.QueryPermission(channel, sender) <= 1)
                {
                    return new CommandResult(false, $"{sender} You need minimum permission of level 2 to close votepoll!");
                }
                return ClosePoll();
            }
            else if(parts.Length > 2 && parts[1].Equals("new")) {
                if (permissionManager.QueryPermission(channel, sender) <= 1)
                {
                    return new CommandResult(false, $"{sender} You need minimum permission of level 2 to start votepoll!");
                }
                return StartPoll(string.Join(" ", parts, 2, parts.Length - 2).Split("|"));
            }
            else if(parts.Length == 2)
            {
                return Vote(parts[1], sender);
            }
            return new CommandResult(false, Help());
        }
    }
}
