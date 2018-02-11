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
            using (var bot = new ChatBot())
            {
                bot.WaitForExit();
            }
        }
    }
}
