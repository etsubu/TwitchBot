using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TwitchBot
{
    /// <summary>
    /// Minimalistic IRC implementation
    /// </summary>
    class IRC
    {
        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer;
        private Thread listenerThread;
        public delegate void MessageReceived(ChatMessage message);
        public event MessageReceived MessageReceivedEvent;

        /// <summary>
        /// Initializes the IRC object
        /// </summary>
        public IRC()
        {

        }

        /// <summary>
        /// Tries to connect to the IRC server
        /// </summary>
        /// <param name="host">Address of the server</param>
        /// <param name="port">Port to connect to</param>
        /// <returns></returns>
        public bool ConnectServer(string host, int port, string user, string password)
        {
            if (this.client != null && this.client.Connected)
                return false;
            this.client = new TcpClient(host, port);
            this.reader = new StreamReader(this.client.GetStream());
            this.writer = new StreamWriter(this.client.GetStream());
            SendMessage("PASS " + password);
            SendMessage("USER " + user + " 0 * :...");
            SendMessage("NICK " + user);
            this.listenerThread = new Thread(MessageReader);
            this.listenerThread.Start();
            return true;
        }

        /// <summary>
        /// Disconnects from the IRC server
        /// </summary>
        public void Disconnect()
        {
            this.writer.Write("QUIT");
            this.writer.Flush();
            this.client.Close();
            this.listenerThread.Join();
        }

        /// <summary>
        /// Sends a raw message to the IRC server. Adds newline to the end
        /// </summary>
        /// <param name="message">IRC message to send</param>
        public void SendMessage(string message)
        {
            this.writer.Write(message + "\r\n");
            this.writer.Flush();
        }

        /// <summary>
        /// Sends a message to the given channel
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="channel">Channel to send the message to</param>
        public void SendMessage(string message, string channel)
        {
            this.writer.Write("PRIVMSG " + channel + " :" + message + "\r\n");
            this.writer.Flush();
        }
        /// <summary>
        /// Reads all the incoming IRC messages
        /// </summary>
        public void MessageReader()
        {
            while(true)
            {
                string line = this.reader.ReadLine();
                Console.WriteLine(line);
                ChatMessage message = ParseMessage(line);
                // Respond to pings
                if (message.GetCommand().Equals("PING"))
                {
                    SendMessage("PONG " + line.Substring(line.IndexOf("PING") + 5));
                }
                else {
                    this.MessageReceivedEvent.Invoke(message);
                }
            }
        }

        /// <summary>
        /// Tries to join a channel
        /// </summary>
        /// <param name="channel">Name of the channel to join</param>
        public void JoinChannel(string channel)
        {
            this.writer.Write("JOIN " + channel + "\r\n");
            this.writer.Flush();
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
                prefixEnd = rawMessage.IndexOf(" ");
                prefix = rawMessage.Substring(1, prefixEnd);
            }
            int trailingStart = rawMessage.IndexOf(" :");
            // Check if trailing is present
            if(trailingStart >= 0)
            {
                trailing = rawMessage.Substring(trailingStart + 2);
            }
            else
            {
                trailingStart = rawMessage.Length;
            }
            // Use trailing start position and prefix end position to get parameters and command
            string cmdAndParameters = rawMessage.Substring(prefixEnd + 1, trailingStart - prefixEnd - 1);
            int parametersStart = cmdAndParameters.IndexOf(" ");
            // Check if parameters exist
            if(parametersStart != -1)
            {
                int parameterCount = 0;
                command = cmdAndParameters.Substring(0, parametersStart);
                string tempParams = cmdAndParameters.Substring(parametersStart + 1);
                LinkedList<string> parametersList = new LinkedList<string>();
                // Extract the parameters
                while((parametersStart = tempParams.IndexOf(" ")) != -1)
                {
                    parameterCount++;
                    parametersList.AddLast(tempParams.Substring(0, parametersStart));
                    tempParams = tempParams.Substring(parametersStart + 1);
                }
                parameters = parametersList.ToArray();
            } else
            {
                command = cmdAndParameters;
            }
            return new ChatMessage(prefix, command, trailing, parameters);
        }
    }
}
