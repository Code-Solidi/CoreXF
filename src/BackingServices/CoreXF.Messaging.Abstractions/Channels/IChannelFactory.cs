/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions.Messages;

using System.Collections.Generic;

namespace CoreXF.Messaging.Abstractions.Channels
{
    public interface IChannelFactory
    {
        IDictionary<string, ICollection<IMessage>> MessageQueue { get; }

        IEnumerable<IMessage> GetAllMessages();

        IFireAndForgetChannel CreateFireAndForgetChannel(IMessageBroker broker);

        IPublishSubscribeChannel CreatePublishSubscribeChannel(IMessageBroker broker);

        IRequestResponseChannel CreateRequestResponseChannel(IMessageBroker broker);
    }
}