using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Messages;

using System;

namespace CoreXF.Messaging.Messages
{
    public class RequestMessage : AbstractMessage, IRequestMessage
    {
        public RequestMessage(string messageType, object payload) : base(messageType, payload)
        {
        }
    }
}