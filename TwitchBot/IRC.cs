using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace TwitchBot
{
    /// <summary>
    /// Minimalistic thread-safe IRC implementation
    /// </summary>
    internal class IRC : IDisposable
    {
        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer;
        private Thread listenerThread;
        private Dictionary<ChannelName, List<Action<ChatMessage>>> callbacks;
        // These are updated everytime ConnectServer is called
        // We save these so we can reconnect if connection is dropped
        private string host;
        private int port;
        public string user;
        private string password;

        /// <summary>
        /// Initialises a new IRC instance
        /// </summary>
        public IRC()
        {
            this.callbacks = new Dictionary<ChannelName, List<Action<ChatMessage>>>();
        }

        /// <summary>
        /// Disposes the IRC instance. If the instance is connected to an IRC server, Disconnect() will be called
        /// </summary>
        public void Dispose()
        {
            if (client != null && client.Connected)
                Disconnect();

            client?.Dispose();
            reader?.Dispose();
            writer?.Dispose();
        }

        /// <summary>
        /// Registers a callback for the messages of the given channel
        /// </summary>
        /// <param name="callback">Callback to invoke for messages</param>
        /// <param name="channel">Channel to register the callback for</param>
        /// <returns></returns>
        public bool RegisterMessageCallback(Action<ChatMessage> callback, ChannelName channel)
        {
            lock(callbacks)
            {
                if(!callbacks.ContainsKey(channel))
                {
                    Console.WriteLine("registered " + channel);
                    callbacks[channel] = new List<Action<ChatMessage>>();
                }
                callbacks[channel].Add(callback);
            }
            return true;
        }

        /// <summary>
        /// Unregisters the given callback from the channel
        /// </summary>
        /// <param name="callback">Callback to unregister</param>
        /// <param name="channel">Channel the callback was attached to</param>
        /// <returns></returns>
        public bool UnregisterMessageCallback(Action<ChatMessage> callback, ChannelName channel)
        {
            lock(callbacks)
            {
                if (callbacks.ContainsKey(channel))
                    callbacks[channel].Remove(callback);
            }
            return true;
        }

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

            try
            {
                client = new TcpClient(host, port);
                reader = new StreamReader(client.GetStream());
                writer = new StreamWriter(client.GetStream());
            }
            catch (Exception)
            {
                return false;
            }

            SendMessage("PASS " + password);
            SendMessage("USER " + user + " 0 * :...");
            SendMessage("NICK " + user);

            this.host = host;
            this.port = port;
            this.user = user;
            this.password = password;
            listenerThread = new Thread(MessageReader);
            listenerThread.Start();


            return true;
        }

        /// <summary>
        /// Synchronously waits for the listener thread to exit
        /// </summary>
        public void WaitForExit() => listenerThread.Join();

        /// <summary>
        /// Disconnects from the IRC server
        /// </summary>
        public void Disconnect()
        {
            lock (writer)
            {
                writer.Write("QUIT");
                writer.Flush();
            }
            lock(client)
            {
                client.Close();
            }
            listenerThread.Join();
        }

        /// <summary>
        /// Sends a raw message to the IRC server. Adds newline to the end
        /// </summary>
        /// <param name="message">IRC message to send</param>
        public void SendMessage(string message)
        {
            lock (writer)
            {
                writer.Write($"{message}\r\n");
                writer.Flush();
            }
        }

        /// <summary>
        /// Sends a message to the given channel
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="channel">Channel to send the message to</param>
        public void SendMessage(string message, ChannelName channel)
        {
            lock (writer)
            {
                Console.WriteLine($"PRIVMSG {channel} :{message}\r\n");
                writer.Write($"PRIVMSG {channel} :{message}\r\n");
                writer.Flush();
            }
        }

        private bool Reconnect()
        {
            if (client != null)
                client.Close();
            Console.WriteLine("Connection dropped. Attempting to reconnect...");
            while (true)
            {
                try
                {
                    client = new TcpClient(host, port);
                    reader = new StreamReader(client.GetStream());
                    writer = new StreamWriter(client.GetStream());
                    break;
                }
                catch (SocketException)
                {
                    // Sleep for 2 seconds before attempting to reconnect
                    Thread.Sleep(2000);
                }
                catch(ArgumentException){
                    // Shouldn't happen
                    return false;
                }
            }
            // We have now reconnected

            SendMessage("PASS " + password);
            SendMessage("USER " + user + " 0 * :...");
            SendMessage("NICK " + user);

            // Rejoin channels
            foreach(ChannelName name in callbacks.Keys)
            {
                JoinChannel(name);
            }
            Console.WriteLine("Reconnected.");
            return true;
    }

        /// <summary>
        /// Reads all the incoming IRC messages
        /// </summary>
        public void MessageReader()
        {
            while (true)
            {
                string line;
                lock (reader)
                {
                    try
                    {
                        line = reader.ReadLine();
                    } catch(IOException)
                    {
                        // Connection lost. Invoke reconnection
                        if (Reconnect())
                            continue;
                        return;
                    }
                }
                Console.WriteLine(line);
                IRCMessage message = ParseMessage(line);

                // Respond to pings
                if (message.Command.Equals("PING"))
                    SendMessage($"PONG {line.Substring(line.IndexOf("PING", StringComparison.Ordinal) + 5)}");
                else if(message.Command.Equals("PRIVMSG"))
                {
                    ChatMessage chatMessage;
                    try
                    {
                        chatMessage = new ChatMessage(message);
                    } catch(ArgumentException)
                    {
                        continue;
                    }
                    lock (callbacks) {
                        Console.WriteLine(chatMessage.Channel);
                        if (callbacks.ContainsKey(chatMessage.Channel)) {
                            foreach (Action<ChatMessage> callback in callbacks[chatMessage.Channel])
                            {
                                SendMessage(".mods", chatMessage.Channel);
                                callback.Invoke(chatMessage);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tries to join a channel
        /// </summary>
        /// <param name="channel">Name of the channel to join</param>
        public void JoinChannel(ChannelName channel)
        {
            lock (writer)
            {
                writer.Write($"JOIN {channel}\r\n");
                writer.Flush();
            }
        }

        /// <summary>
        /// Leaves the given channel and removes all listeners associated with it
        /// </summary>
        /// <param name="channel"></param>
        public void LeaveChannel(ChannelName channel)
        {
            lock(writer)
            {
                writer.Write($"PART {channel}\r\n");
                writer.Flush();
            }
            lock(callbacks)
            {
                callbacks.Remove(channel);
            }
        }

        /// <summary>
        /// Parses a single IRC message to its corresponding parts
        /// </summary>
        /// <param name="rawMessage">Raw representation of a single IRC message</param>
        /// <returns>IRCMessage object containing the strucuted IRC message</returns>
        private IRCMessage ParseMessage(string rawMessage)
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

                parametersList.AddLast(tempParams);
                parameters = parametersList.ToArray();
            }
            else
                command = cmdAndParameters;

            return new IRCMessage(prefix, command, trailing, parameters);
        }
    }
}
