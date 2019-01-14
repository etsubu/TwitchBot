using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using TwitchBot.Commands.Permissions;

namespace TwitchBot.Commands.MessageFilters
{
    /// <summary>
    /// MessageFilterHandler owns the actual message filter objects and is responsible for using the enabled ones
    /// </summary>
    internal class MessageFilterHandler
    {
        public bool LinksAllowed { get; set; }
        public bool UnicodeAllowed { get; set; }
        public bool CapsAllowed { get; set; }
        private readonly IRC irc;
        private readonly PermissionManager permissions;
        private LinkedList<string> bannedWords;
        private readonly double CapsPercentFilter = 0.80;
        private readonly double UnicodePercentFilter = 0.30;

        // Permits is temporary dictionary that allows the next message that triggers filters from the given user
        // This way mods can permit user to post a single link, or copypasta, or whatever normally triggers filters
        // The permit contains the time it was given and will expire in 1 minute
        // This way the user can't "save" the permit for later date
        public Dictionary<string, DateTimeOffset> permits;

        public MessageFilterHandler(PermissionManager permissions, IRC irc)
        {
            this.irc = irc;
            this.permissions = permissions;
            this.bannedWords = new LinkedList<string>();
            this.permits = new Dictionary<string, DateTimeOffset>();
            LinksAllowed = false;
            UnicodeAllowed = false;
            CapsAllowed = false;
        }

        public CommandResult AddPermit(string user)
        {
            user = user.ToLower();
            DateTimeOffset time = DateTimeOffset.UtcNow;
            List<string> toRemove = new List<string>();
            // Remove expired permits
            lock (permits)
            {
                foreach (string usr in permits.Keys)
                {
                    if(permits[usr].CompareTo(time) <= 0)
                    {
                        toRemove.Add(usr);
                    }
                }
                foreach (string usr in toRemove)
                {
                    permits.Remove(usr);
                }
                if (permits.ContainsKey(user))
                {
                    return new CommandResult(false, $"{user} already has permission to post a link!");
                }
                else
                {
                    permits[user] = time.AddMinutes(1); // Set expire time to current time + 1 minute
                    return new CommandResult(true, $"{user} was given permission to post a link!");
                }
            }
        }

        /// <summary>
        /// Retrieves information whether given user has permission to post a link and removes the permit if it existed
        /// </summary>
        /// <param name="user">User to query</param>
        /// <returns>True if user can post a link, false if not</returns>
        public bool PopPermit(string user)
        {
            user = user.ToLower();
            DateTimeOffset time = DateTimeOffset.UtcNow;
            List<string> toRemove = new List<string>();
            // Remove expired permits
            lock (permits)
            {
                foreach (string usr in permits.Keys)
                {
                    if (permits[usr].CompareTo(time) <= 0)
                    {
                        toRemove.Add(usr);
                    }
                }
                foreach(string usr in toRemove)
                {
                    permits.Remove(usr);
                }
                if(permits.ContainsKey(user))
                {
                    permits.Remove(user);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Calculates the amount of unicode characters in string that are not ASCII or 'ä', 'ö', 'å'
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static double UnicodePercent(string message)
        {
            int unicode = 0;
            foreach(char c in message)
            {
                char lower = Char.ToLower(c);
                int lowerAsInt = lower;
                if (lower != 'ä' && lower != 'ö' && lower != 'å' && lowerAsInt > 255)
                    unicode++;
            }
            return ((double)unicode) / (message.Length);
        }
        /// <summary>
        /// Calculates the amount of uppercase characters in the message
        /// </summary>
        /// <param name="message">Message to analyze</param>
        /// <returns>Amount of uppercase characters in percent value 0-100</returns>
        public static double CapsPercent(string message)
        {
            if (message.Length == 0)
                return 0;
            int caps = 0;
            foreach(char c in message)
            {
                if (Char.IsUpper(c))
                    caps ++;
            }
            return ((double)caps) / (message.Length);
        }

        public static bool IsLink(string msg)
        {
            msg = msg.ToLower().Trim();
            string[] parts = msg.Split(" ");
            foreach(string str in parts)
            {
                if (msg.StartsWith("http://") || msg.StartsWith("https://") || msg.StartsWith("ftp://") || msg.StartsWith("www."))
                    return true;
                return Regex.IsMatch(str, "\\w+\\.\\w+");
            }
            return false;
        }

        public bool ProcessFilters(ChatMessage message)
        {
            if(!LinksAllowed && (permissions.QueryPermission(message.Channel, message.Sender) < 1 || !PopPermit(message.Sender)) && IsLink(message.Message))
            {
                irc.SendMessage($"/timeout {message.Sender} 1", message.Channel);
                irc.SendMessage($"{message.Sender} Links are not allowed!", message.Channel);
                return true;
            }
            if(message.Message.Length > 10)
            {
                if(CapsPercent(message.Message) > CapsPercentFilter && (permissions.QueryPermission(message.Channel, message.Sender) < 1 || !PopPermit(message.Sender)))
                {
                    irc.SendMessage($"/timeout {message.Sender} 1", message.Channel);
                    irc.SendMessage($"{message.Sender} Calm down mate. Let's not spam caps!", message.Channel);
                    return true;
                }
                else if(UnicodePercent(message.Message) > UnicodePercentFilter && (permissions.QueryPermission(message.Channel, message.Sender) < 1 || !PopPermit(message.Sender)))
                {
                    irc.SendMessage($"/timeout {message.Sender} 1", message.Channel);
                    irc.SendMessage($"{message.Sender} hmm it seems like someone is posting pastaThat in the chat", message.Channel);
                    return true;
                }
            }
            return false;
        }
    }
}
