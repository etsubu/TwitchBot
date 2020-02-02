using System;
using System.Collections.Generic;
using System.IO;

namespace TwitchBot
{
    /// <summary>
    /// Initializes the program
    /// </summary>
    internal class Program
    {

        /// <summary>
        /// Builds a new configuration file by asking the settings from user
        /// </summary>
        /// <param name="configPath">File path to write the config to</param>
        /// <returns>True if new config file was written</returns>
        private static bool InitializeConfiguration(string configPath)
        {
            Console.Write("Would you like to initialize configuration now (y/n): ");
            string response = Console.ReadLine();
            if (response.ToLower().Equals("y"))
            {
                Console.WriteLine("The program can support multiple bot instances with different user names. Let's define those first");
                List<User> users = new List<User>();
                while (true)
                {
                    User user = new User();
                    Console.Write("Twitch username for bot (empty will stop adding users): ");
                    user.Username = Console.ReadLine();
                    if (user.Username.Length == 0)
                        break;
                    Console.Write("Twitch oauth token for the bot (empty will stop adding users): ");
                    user.Oauth = Console.ReadLine().Trim();
                    if (user.Oauth.Length == 0)
                        break;
                    users.Add(user);
                }
                if (users.Count == 0)
                    return false;
                Console.Write("Input the twitch username of the person who owns the bot (will receive global admin privileges): ");
                string owner = Console.ReadLine();
                if (owner.Length == 0)
                    return false;
                Console.Write("Do you wish to configure the twitch irc hostname and port. If not sure then leave empty and defaults will be used (y/n): ");
                response = Console.ReadLine();
                Connection connection = new Connection();
                if (response.ToLower().Equals("y"))
                {
                    Console.Write("Input hostname: ");
                    connection.Host = Console.ReadLine();
                    int port;
                    Console.Write("Input port number: ");
                    if (!int.TryParse(Console.ReadLine(), out port) || port < 1 || port > 65536)
                    {
                        Console.WriteLine("Given input was not a valid port number [1-65536]");
                        return true;
                    }
                    connection.Port = port;
                }
                else
                {
                    connection.Host = "irc.twitch.tv";
                    connection.Port = 6667;
                }
                Configuration config = new Configuration(users, owner, connection);
                try
                {
                    Console.WriteLine("Writing config to file: " + configPath);
                    File.WriteAllText(configPath, Configuration.SerializeConfiguration(config));
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to write config: " + e.Message);
                }
            }
            return false;
        }

        /// <summary>
        /// Main method of the program that starts the bot with default config file path if none was given as argument
        /// </summary>
        /// <param name="args">Config file path (optional)</param>
        static void Main(string[] args)
        {
            string configPath = "config.yaml";
            if(args.Length > 0)
            {
                // Use the given config file path
                configPath = args[0];
            }
            Database database = new Database();
            ChatBot bot;
            try
            {
                //bot = new ChatBot(Configuration.LoadFromJson(configPath), database);
                bot = new ChatBot(Configuration.InitializeConfigurationFromFile(configPath), database);
            } catch(Exception e)
            {
                Console.WriteLine("Failed to initialize bot instance " + e.Message);
                InitializeConfiguration(configPath);
                return;
            }
            using (bot)
            {
                bot.WaitForExit();
            }
        }
    }
}
