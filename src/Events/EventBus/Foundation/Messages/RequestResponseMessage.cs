using EventBus.Interfaces;
using System;

namespace EventBus.Foundation.Messages
{
    public class RequestResponseMessage : AbstractMessage, IRequestResponseMessage
    {
        public RequestResponseMessage(string recipient, string callback = null)
        {
            this.Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
            this.Callback = callback;
        }

        public Uri Sender { get; }

        public string Recipient { get; }

        public string Callback { get; set; }
    }
}