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

        /// <summary>
        /// Tries to initialize the IRC connection
        /// </summary>
        /// <param name="host">Address of the IRC server</param>
        /// <param name="port">Port to connect to</param>
        public IRC(string host, int port)
        {
            this.socket = new Socket(SocketType.Stream, ProtocolType.IPv4);
            this.socket.Connect(host, port);
        }
    }
}
