using CoreXF.Messaging.Abstractions.Messages;

using System;

namespace CoreXF.Messaging.Messages
{
    public class RequestResponseMessage : AbstractMessage, IRequestResponseMessage
    {
        public RequestResponseMessage(string messageType, object payload) : base(messageType, payload)
        {
        }
    }
}