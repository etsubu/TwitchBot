using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBot
{
    /// <summary>
    /// Defines the interface for listening received IRC messages
    /// </summary>
    interface IMessageListener
    {
        /// <summary>
        /// Called when a message is received over IRC
        /// </summary>
        /// <param name="message">The actual content of the message</param>
        /// <param name="sender">Sender of the message</param>
        /// <param name="channel">What channel it was sent on</param>
        void MessageReceived(string message, string sender, string channel);
    }
}
