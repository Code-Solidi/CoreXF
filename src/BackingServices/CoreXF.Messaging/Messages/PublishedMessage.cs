using CoreXF.Messaging.Abstractions.Messages;

using System;

namespace CoreXF.Messaging.Messages
{
    public class PublishedMessage : AbstractMessage, IPublishedMessage
    {
        //public PublishSubscribeMessage(string messageType) : base(messageType)
        //{
        //}

        //public PublishSubscribeMessage(IPublishSubscribeMessage message) : base(message.Type)
        //{
        //    _ = message as AbstractMessage ?? throw new ArgumentNullException(nameof(message));
        //    this.DateTime = message.DateTime;
        //    this.Type = message.Type;
        //    this.SetPayload(message);
        //}

        public PublishedMessage(string messageType, object payload) : base(messageType, payload)
        {
        }
    }
}