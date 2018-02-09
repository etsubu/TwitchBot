using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace TwitchBot
{
    /// <summary>
    /// Minimalistic IRC implementation
    /// </summary>
    class IRC
    {
        private Socket socket;
        private LinkedList<IMessageObserver> observers;

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
            return true;
        }

        /// <summary>
        /// Tries to join a channel
        /// </summary>
        /// <param name="channel">Name of the channel to join</param>
        public void JoinChannel(string channel)
        {
            this.socket.Send(System.Text.Encoding.UTF8.GetBytes("JOIN " + channel + "\r\n"));
        }
    }
}
