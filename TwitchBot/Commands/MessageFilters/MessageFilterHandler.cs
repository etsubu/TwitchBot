using System;
using System.Collections.Generic;
using System.Text;
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
        private readonly double CapsPercentFilter = 95;
        private readonly double UnicodePercentFilter = 95;

        public MessageFilterHandler(PermissionManager permissions, IRC irc)
        {
            this.irc = irc;
            this.permissions = permissions;
            this.bannedWords = new LinkedList<string>();
            LinksAllowed = false;
            UnicodeAllowed = false;
            CapsAllowed = false;
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
            return msg.StartsWith("http://") || msg.StartsWith("https://") || msg.StartsWith("ftp://") || msg.StartsWith("www.");
        }

        public bool ProcessFilters(ChatMessage message)
        {
            if(!LinksAllowed && permissions.QueryPermission(message.Channel, message.Sender) < 1 && IsLink(message.Message))
            {
                irc.SendMessage("/timeout " + message.Sender + " 1");
                irc.SendMessage($"{message.Sender} Links are not allowed!", message.Channel);
                return true;
            }
            if(message.Message.Length > 10)
            {
                if(CapsPercent(message.Message) > CapsPercentFilter)
                {
                    irc.SendMessage("/timeout " + message.Sender + " 1");
                    irc.SendMessage($"{message.Sender} Calm down mate. Let's not spam caps!", message.Channel);
                    return true;
                }
                else if(UnicodePercent(message.Message) > UnicodePercentFilter)
                {
                    irc.SendMessage($"{message.Sender} hmm it seems like someone is posting pastaThat in the chat", message.Channel);
                    return true;
                }
            }
            return false;
        }
    }
}
