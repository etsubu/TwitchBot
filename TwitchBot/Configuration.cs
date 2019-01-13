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

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TwitchBot
{
    internal struct User
    {
        public string Username { get; set; }
        public string Oauth { get; set; }
    }

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

        public List<User> Users { get; }
        public string Owner { get; }
        public Connection Connection { get; }

        public Configuration(List<User> users, string owner, Connection connection)
        {
            this.Users = users;
            Owner = owner.ToLower();
            Connection = connection;
        }
    }
}