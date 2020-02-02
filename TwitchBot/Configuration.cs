using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TwitchBot
{
    /// <summary>
    /// User account details
    /// </summary>
    public class User
    {
        public string Username { get; set; }
        public string Oauth { get; set; }

        public User(string username, string oauth)
        {
            Username = username;
            Oauth = oauth;
        }

        public User()
        {

        }
    }

    /// <summary>
    /// Connection details to the server
    /// </summary>
    public class Connection
    {
        public string Host { get; set; }
        public int Port { get; set; }

        public Connection(string host, int port)
        {
            Host = host;
            Port = port;
        }

        public Connection()
        {

        }
    }

    /// <summary>
    /// Configuration of the bot containing user accounts and connection details 
    /// </summary>
    internal class Configuration
    {

        public List<User> Users { get; set; }
        public string Owner { get; set; }
        public Connection Connection { get; set; }

        /// <summary>
        /// Loads configuration from file
        /// </summary>
        /// <param name="path">Path to config file</param>
        /// <returns>Configuration object</returns>
        public static Configuration InitializeConfigurationFromFile(string path)
        {
            string fileContent = File.ReadAllText(path);
            var input = new StringReader(fileContent);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<Configuration>(input);
        }

        /// <summary>
        /// Serializes given configuration to yaml format as string
        /// </summary>
        /// <param name="c">Configuration to serialize</param>
        /// <returns>Serialized configuration as string</returns>
        public static string SerializeConfiguration(Configuration c)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            return serializer.Serialize(c);
        }

        /// <summary>
        /// Initializes Configuration object
        /// </summary>
        /// <param name="users">List of user accounts that will be hosting separate bost instances</param>
        /// <param name="owner">Owner of this bot</param>
        /// <param name="connection">Connection details to the twitch server</param>
        public Configuration(List<User> users, string owner, Connection connection)
        {
            Users = users;
            Owner = owner.ToLower();
            Connection = connection;
        }

        public Configuration()
        {

        }
    }
}