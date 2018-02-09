using System;
using System.Collections.Generic;
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
        private Socket socket;
        private LinkedList<IMessageObserver> observers;
        private Thread listenerThread;

        /// <summary>
        /// Initializes the IRC object
        /// </summary>
        /// <param name="observer">Initial observer to receive messages from the IRC server</param>
        public IRC(IMessageObserver observer)
        {
            this.observers = new LinkedList<IMessageObserver>();
            this.observers.AddLast(observer);
        }

        /// <summary>
        /// Adds a new observer to receive messages from the IRC server
        /// </summary>
        /// <param name="observer">Observer to register</param>
        public void RegisterObserver(IMessageObserver observer)
        {
            lock(this.observers)
            {
                this.observers.AddLast(observer);
            }
        }

        /// <summary>
        /// Unregisters the given observer so it won't receive further messages from the IRC server
        /// </summary>
        /// <param name="observer">Observer to unregister</param>
        /// <returns>True if the observer was unregister and false if it did not exist</returns>
        public bool UnregisterObserver(IMessageObserver observer)
        {
            lock(this.observers)
            {
                return this.observers.Remove(observer);
            }
        }

        /// <summary>
        /// Tries to connect to the IRC server
        /// </summary>
        /// <param name="host">Address of the server</param>
        /// <param name="port">Port to connect to</param>
        /// <returns></returns>
        public bool ConnectServer(string host, int port)
        {
            if (this.socket.Connected)
                return false;
            this.socket = new Socket(SocketType.Stream, ProtocolType.IPv4);
            this.socket.Connect(host, port);
            this.listenerThread = new Thread(MessageReader);
            this.listenerThread.Start();
            return true;
        }

        public void MessageReader()
        {
            string previous = "";
            while(true)
            {
                byte[] buffer = new byte[256];
                int len = this.socket.Receive(buffer);
                string rawMessage = previous + Encoding.UTF8.GetString(buffer);
                int messageEnd = rawMessage.IndexOf("\r\n");
                if(messageEnd == -1)
                {
                    previous = rawMessage;
                    continue;
                }
                else if(messageEnd < rawMessage.Length - 3)
                {
                    previous = rawMessage.Substring(messageEnd + 2);
                }
                else
                {
                    previous = "";
                }
                ChatMessage message = ParseMessage(rawMessage.Substring(0, messageEnd));

            }
        }

        /// <summary>
        /// Tries to join a channel
        /// </summary>
        /// <param name="channel">Name of the channel to join</param>
        public void JoinChannel(string channel)
        {
            this.socket.Send(Encoding.UTF8.GetBytes("JOIN " + channel + "\r\n"));
        }

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
            return null;
        }
    }
}
