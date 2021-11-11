/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions.Messages;

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