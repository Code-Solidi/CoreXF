using System;

namespace EventBus.Interfaces
{
    public interface IRequestResponseMessage : IMessage
    {
        /// <summary>
        /// Gets the recipient Identity.
        /// </summary>
        /// <value>
        /// The recipient Identity.
        /// </value>
        string Recipient { get; }

        /// <summary>
        /// Gets or sets the sender.
        /// </summary>
        /// <value>
        /// The sender.
        /// </value>
        //Uri Sender { get; }

        /// <summary>
        /// Gets or sets the callback.
        /// </summary>
        /// <value>
        /// The callback.
        /// </value>
        string Callback { get; set; }
    }
}