/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Channels;
using CoreXF.Messaging.Abstractions.Messages;

using Microsoft.Extensions.Logging;

using System.Linq;

namespace CoreXF.Messaging.Channels.InProcess
{
    public class InProcessPublishSubscriberChannel : AbstractChannel, IPublishSubscribeChannel
    {
        private readonly IMessageBroker broker;

        internal InProcessPublishSubscriberChannel(AbstractChannelFactory factory, IMessageBroker broker, ILogger logger)
            : base(factory, logger)
        {
            this.broker = broker;
        }

        public void Publish(IPublishedMessage message)
        {
            this.Logger.LogInformation($"Publishing event '{message.Id}' of type '{message.Type}'.");
            var subscribers = (this.broker as MessageBroker).GetSubscribers(message.Type);
            foreach (var subscriber in subscribers)
            {
                subscriber.Recieve(message);
            }

            this.Logger.LogInformation($"{subscribers.Count()} subscribers notified.");
        }
    }
}