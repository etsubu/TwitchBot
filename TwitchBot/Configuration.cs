﻿/*
 * Sample JSON configuration file:
 *  {
        "username": "",
        "oauth": "oauth:---------------------",
        "connection": {
            "host": "irc.twitch.tv",
            "port": 6667
        },
        "channels": [
            ""
        ]
        "globalpermission": [
            "name": permission
        ]
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
        public Connection Connection { get; }
        public string[] Channels { get; }

        public Configuration(string username, string oauthToken, Connection connection, string[] channels)
        {
            Username = username;
            OAuthToken = oauthToken;
            Connection = connection;
            Channels = channels;
        }
    }
}