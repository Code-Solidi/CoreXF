/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Channels;
using CoreXF.Messaging.Abstractions.Messages;

using Microsoft.Extensions.Logging;

using System;
using System.Linq;

namespace CoreXF.Messaging.Channels.InProcess
{
    /// <summary>
    /// The in process publish subscriber channel.
    /// </summary>
    public class InProcessPublishSubscribeChannel : AbstractChannel, IPublishSubscribeChannel
    {
        /// <summary>
        /// The broker.
        /// </summary>
        private readonly IMessageBroker broker;

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcessPublishSubscribeChannel"/> class.
        /// </summary>
        /// <param name="broker">The broker.</param>
        /// <param name="logger">The logger.</param>
        internal InProcessPublishSubscribeChannel(IMessageBroker broker, ILogger logger) : base(logger)
        {
            this.broker = broker;
        }

        /// <summary>
        /// TODO: Add Summary
        /// </summary>
        /// <param name="message">The message.</param>
        public void Publish(IPublishedMessage message)
        {
            this.Logger?.LogInformation($"Publishing message '{message.Id}' of type '{message.Type}'.");
            var subscribers = (this.broker as MessageBroker)?.GetSubscribers(message.Type) ?? Array.Empty<ISubscriber>();
            foreach (var subscriber in subscribers)
            {
                subscriber.Receive(message);
            }

            this.Logger?.LogInformation($"{subscribers.Count()} subscribers notified.");
        }
    }
}