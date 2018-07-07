using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot.Commands
{
    /// <summary>
    /// Defines a single BroadcastMessage
    /// </summary>
    internal class BroadcastMessage
    {
        private readonly int delay;
        private readonly string message;

        /// <summary>
        /// Initializes BroadcastMessage
        /// </summary>
        /// <param name="delay">Delay in seconds for this message to be sent</param>
        /// <param name="message">Message to send</param>
        public BroadcastMessage(int delay, string message)
        {
            this.delay = delay;
            this.message = message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The delay in seconds for this message to be sent</returns>
        public int GetDelay()
        {
            return delay;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The broadcasted message</returns>
        public string GetMessage()
        {
            return message;
        }

        /// <summary>
        /// Compares the given tick count to the delay of the message to determine if the broadcast message should be sent
        /// </summary>
        /// <param name="ticks">Current tick count</param>
        /// <returns>True if this message should be sent, false if not</returns>
        public bool IsTiming(int ticks) => ticks % delay == 0;
    }
}
