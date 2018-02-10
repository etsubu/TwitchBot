using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace TwitchBot
{
    /// <summary>
    /// Minimalistic IRC implementation
    /// </summary>
    internal class IRC
    {
        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer;
        private Thread listenerThread;
        public delegate void MessageReceived(ChatMessage message);
        public event MessageReceived MessageReceivedEvent;

        /// <summary>
        /// Tries to connect to the IRC server
        /// </summary>
        /// <param name="host">Address of the server</param>
        /// <param name="port">Port to connect to</param>
        /// <param name="user">Username to use</param>
        /// <param name="password">Password of the user</param>
        /// <returns></returns>
        public bool ConnectServer(string host, int port, string user, string password)
        {
            if (client != null && client.Connected)
                return false;

            client = new TcpClient(host, port);
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());

            SendMessage("PASS " + password);
            SendMessage("USER " + user + " 0 * :...");
            SendMessage("NICK " + user);

            listenerThread = new Thread(MessageReader);
            listenerThread.Start();

            return true;
        }

        /// <summary>
        /// Disconnects from the IRC server
        /// </summary>
        public void Disconnect()
        {
            writer.Write("QUIT");
            writer.Flush();
            client.Close();
            listenerThread.Join();
        }

        /// <summary>
        /// Sends a raw message to the IRC server. Adds newline to the end
        /// </summary>
        /// <param name="message">IRC message to send</param>
        public void SendMessage(string message)
        {
            writer.Write($"{message}\r\n");
            writer.Flush();
        }

        /// <summary>
        /// Sends a message to the given channel
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="channel">Channel to send the message to</param>
        public void SendMessage(string message, string channel)
        {
            writer.Write($"PRIVMSG {channel} :{message}\r\n");
            writer.Flush();
        }

        /// <summary>
        /// Reads all the incoming IRC messages
        /// </summary>
        public void MessageReader()
        {
            while(true)
            {
                string line = reader.ReadLine();
                Console.WriteLine(line);
                ChatMessage message = ParseMessage(line);

                // Respond to pings
                if (message.Command.Equals("PING"))
                    SendMessage($"PONG {line.Substring(line.IndexOf("PING", StringComparison.Ordinal) + 5)}");
                else
                    MessageReceivedEvent?.Invoke(message);
            }
        }

        /// <summary>
        /// Tries to join a channel
        /// </summary>
        /// <param name="channel">Name of the channel to join</param>
        public void JoinChannel(string channel)
        {
            writer.Write($"JOIN {channel}\r\n");
            writer.Flush();
        }

        /// <summary>
        /// Parses a single IRC message to its corresponding parts
        /// </summary>
        /// <param name="rawMessage">Raw representation of a single IRC message</param>
        /// <returns>ChatMessage object containing the strucuted IRC message</returns>
        private ChatMessage ParseMessage(string rawMessage)
        {
            string prefix = "", trailing = "", command = "";
            string[] parameters = null;
            int prefixEnd = -1;

            // Check if prefix is present
            if(rawMessage[0] == ':')
            {
                prefixEnd = rawMessage.IndexOf(" ", StringComparison.Ordinal);
                prefix = rawMessage.Substring(1, prefixEnd);
            }

            int trailingStart = rawMessage.IndexOf(" :", StringComparison.Ordinal);
            // Check if trailing is present
            if (trailingStart >= 0)
                trailing = rawMessage.Substring(trailingStart + 2);
            else
                trailingStart = rawMessage.Length;

            // Use trailing start position and prefix end position to get parameters and command
            string cmdAndParameters = rawMessage.Substring(prefixEnd + 1, trailingStart - prefixEnd - 1);
            int parametersStart = cmdAndParameters.IndexOf(" ", StringComparison.Ordinal);

            // Check if parameters exist
            if (parametersStart != -1)
            {
                int parameterCount = 0;
                command = cmdAndParameters.Substring(0, parametersStart);
                string tempParams = cmdAndParameters.Substring(parametersStart + 1);
                LinkedList<string> parametersList = new LinkedList<string>();

                // Extract the parameters
                while ((parametersStart = tempParams.IndexOf(" ", StringComparison.Ordinal)) != -1)
                {
                    parameterCount++;
                    parametersList.AddLast(tempParams.Substring(0, parametersStart));
                    tempParams = tempParams.Substring(parametersStart + 1);
                }

                parameters = parametersList.ToArray();
            }
            else
                command = cmdAndParameters;

            return new ChatMessage(prefix, command, trailing, parameters);
        }
    }
}
