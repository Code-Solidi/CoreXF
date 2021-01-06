using CoreXF.Messaging.Abstractions.Messages;

using System;

namespace CoreXF.Messaging.Messages
{
    public class RequestResponseMessage : AbstractMessage, IRequestResponseMessage
    {
        public RequestResponseMessage(string recipient, string messageType, object payload) : base(messageType, payload)
        {
            this.Recipient = recipient;
        }

        public Uri Sender { get; }

        public string Recipient { get; }
    }
}