/*
 * Sample JSON configuration file:
 *  {
        "username": "",
        "oauth": "oauth:---------------------",
        "owner": "",
        "connection": {
            "host": "irc.twitch.tv",
            "port": 6667
        },
    }
 */

using System.IO;
using Newtonsoft.Json;

namespace TwitchBot
{
    internal struct Connection
    {
        public string Host;
        public int Port;

        public Connection(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }

    internal class Configuration
    {
        public static Configuration LoadFromJson(string path) => JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(path));

        public string Username { get; }
        [JsonProperty("oauth")]
        public string OAuthToken { get; }
        public string Owner { get; }
        public Connection Connection { get; }

        public Configuration(string username, string oauthToken, string owner, Connection connection)
        {
            Username = username.ToLower();
            OAuthToken = oauthToken;
            Owner = owner.ToLower();
            Connection = connection;
        }
    }
}