using System;

namespace TwitchBot
{
    /// <summary>
    /// Initializes the program
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main method of the program
        /// </summary>
        /// <param name="args">Currently unused</param>
        static void Main(string[] args)
        {
            Database database = new Database();
            /*database.QueryPermissions();
            if (database.AddPermission(new Commands.Permissions.ChannelUsernamePair("#nagrodus", "nagrodus", false), 10))
                Console.WriteLine("done");
            Console.Read();
            */
            using (var bot = new ChatBot(Configuration.LoadFromJson("config.json"), database))
            {
                bot.WaitForExit();
            }
        }
    }
}
