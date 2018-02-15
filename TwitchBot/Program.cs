using Microsoft.Extensions.DependencyInjection;
using TwitchBot.Commands;
using TwitchBot.Commands.Permissions;

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
            var config = Configuration.LoadFromJson("config.json");

            var services = new ServiceCollection()
                .AddSingleton(config)
                .AddSingleton<IRC>()
                .AddSingleton<ChatBot>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<PermissionManager>();

            var provider = services.BuildServiceProvider();

            var chatBot = provider.GetRequiredService<ChatBot>();
            chatBot.Start();
            chatBot.WaitForExit();
        }
    }
}
