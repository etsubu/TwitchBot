using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot
{
    /// <summary>
    /// Defines the interface for listening received IRC messages
    /// </summary>
    interface IMessageObserver
    {
        /// <summary>
        /// Called when a message is received over IRC
        /// </summary>
        /// <param name="message">ChatMessage that contains all the information about the received message</param>
        void MessageReceived(ChatMessage message);
    }
}
