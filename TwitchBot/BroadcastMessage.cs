using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot
{
    /// <summary>
    /// Defines a single BroadcastMessage
    /// </summary>
    class BroadcastMessage
    {
        private int delay;
        private string message;

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
            return this.delay;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The broadcasted message</returns>
        public string GetMessage()
        {
            return this.message;
        }

        /// <summary>
        /// Compares the given tick count to the delay of the message to determine if the broadcast message should be sent
        /// </summary>
        /// <param name="ticks">Current tick count</param>
        /// <returns>True if this message should be sent, false if not</returns>
        public bool IsTiming(int ticks)
        {
            return (ticks % this.delay) == 0;
        }
    }
}
