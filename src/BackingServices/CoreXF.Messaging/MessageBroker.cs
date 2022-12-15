/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Channels;
using CoreXF.Messaging.Abstractions.Messages;
using CoreXF.Messaging.Messages;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CoreXF.Messaging
{
    /// <summary>
    /// The message broker.
    /// </summary>
    public class MessageBroker : IMessageBroker
    {
        private readonly ILogger logger;
        private readonly IFireAndForgetChannel fireForgetChannel;
        private readonly IPublishSubscribeChannel publishSubscribeChannel;
        private readonly IRequestResponseChannel requestResponseChannel;
        private readonly IDictionary<string, List<ISubscriber>> subscribers;
        private readonly IDictionary<string, IRecipient> recipients;
        private readonly object locker = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBroker"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public MessageBroker(AbstractChannelFactory factory)
        {
            _ = factory ?? throw new ArgumentNullException(nameof(factory));

            this.subscribers = new Dictionary<string, List<ISubscriber>>();
            this.recipients = new Dictionary<string, IRecipient>();

            this.fireForgetChannel = factory.CreateFireAndForgetChannel(this);
            this.publishSubscribeChannel = factory.CreatePublishSubscribeChannel(this);
            this.requestResponseChannel = factory.CreateRequestResponseChannel(this);

            this.logger = factory.Logger;
        }

        public event FireEvent OnFire;

        public event ResponseEvent OnResponse;

        #region Fire And Forget

        /// <inheritdoc/>
        public void Fire(IFireAndForgetMessage message)
        {
            this.logger?.LogInformation($"Firing message: {message.Id} ({message.Type}).");
            this.fireForgetChannel.Fire(message);
            this.logger?.LogInformation($"Message fired: {message.Id}");
            this.OnFire?.Invoke(message);
        }

        /// <inheritdoc/>
        public IEnumerable<IFireAndForgetMessage> Peek(string messageType)
        {
            var result = this.fireForgetChannel.Peek(messageType);
            return result.Cast<FireAndForgetMessage>();
        }

        #endregion Fire And Forget

        #region Publish/Subscribe
        
        /// <inheritdoc/>
        [SuppressMessage("Minor Code Smell", "S1125:Boolean literals should not be redundant", Justification = "<Pending>")]
        public void Subscribe(ISubscriber subscriber, string messageType)
        {
            _ = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
            if (string.IsNullOrWhiteSpace(messageType))
            {
                throw new ArgumentException("Message type cannot be null or empty", nameof(subscriber));
            }

            lock (this.locker)
            {
                if (this.subscribers.ContainsKey(messageType) == false)
                {
                    throw new InvalidOperationException($"Message type '{messageType}' has not been registered. Please, register it before subscribing to it.");
                }

                var messageTypeSubscribers = this.subscribers[messageType];
                if (messageTypeSubscribers.Any(x => x.Identity == subscriber.Identity))
                {
                    throw new InvalidOperationException($"'{subscriber.Identity}' has already been subscribed for message type '{messageType}'.");
                }

                this.subscribers[messageType].Add(subscriber);
            }
        }

        /// <inheritdoc/>
        public bool IsSubscribed(ISubscriber subscriber, string messageType)
        {
            lock (this.locker)
            {
                if (this.subscribers.ContainsKey(messageType))
                {
                    return this.subscribers[messageType].Find(x => x.Identity == subscriber.Identity) != default;
                }

                throw new InvalidOperationException($"Message type '{messageType}' has not been registered.");
            }
        }

        /// <inheritdoc/>
        public void Unsubscribe(string identity, string messageType)
        {
            lock (this.locker)
            {
                if (this.subscribers.ContainsKey(messageType))
                {
                    var subscriber = this.subscribers[messageType].SingleOrDefault(s => s.Identity == identity) ??
                        throw new InvalidOperationException($"Cannot unsubscribe, '{identity}' has not been subscribed to '{messageType}'.");

                    this.subscribers[messageType].Remove(subscriber);
                    return;
                }

                throw new InvalidOperationException($"Message type '{messageType}' has not been registered.");
            }
        }

        /// <inheritdoc/>
        [SuppressMessage("Minor Code Smell", "S1125:Boolean literals should not be redundant", Justification = "<Pending>")]
        public void Register(string messageType)
        {
            lock (this.locker)
            {
                if (this.subscribers.ContainsKey(messageType) == false)
                {
                    this.subscribers.Add(messageType, new List<ISubscriber>());
                }
            }
        }

        /// <inheritdoc/>
        [SuppressMessage("Minor Code Smell", "S1125:Boolean literals should not be redundant", Justification = "<Pending>")]
        public void Unregister(string messageType)
        {
            lock (this.locker)
            {
                if (this.subscribers.ContainsKey(messageType) == false)
                {
                    throw new InvalidOperationException($"Message type '{messageType}' has not been registered.");
                }

                if (this.subscribers[messageType].Count != 0)
                {
                    throw new InvalidOperationException($"Subscriber list for message type '{messageType}' is not empty.");
                }

                this.subscribers.Remove(messageType);
            }
        }

        /// <inheritdoc/>
        public bool IsRegistered(string messageType)
        {
            lock (this.locker)
            {
                return this.subscribers.ContainsKey(messageType);
            }
        }

        /// <summary>
        /// Gets the subscribers.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <returns><![CDATA[IEnumerable<ISubscriber>]]></returns>
        internal IEnumerable<ISubscriber> GetSubscribers(string messageType)
        {
            lock (this.locker)
            {
                return this.subscribers.ContainsKey(messageType) ? this.subscribers[messageType] : new List<ISubscriber>();
            }
        }

        /// <inheritdoc/>
        public void Publish(IPublishedMessage message)
        {
            this.publishSubscribeChannel.Publish(message);
        }

        #endregion Publish/Subscribe

        #region Request/Response

        /// <inheritdoc/>
        public IMessageResponse Request(IRequestMessage message)
        {
            _ = message ?? throw new ArgumentNullException(nameof(message));

            this.logger?.LogInformation($"Requesting: {message.Id} ({message.Type}).");
            var response = this.requestResponseChannel.Request(message);
            this.OnResponse?.Invoke(message, response);
            this.logger?.LogInformation($"Response for: {message.Id} ({message.Type}) => '{response.Content}'.");

            return response;
        }

        /// <inheritdoc/>
        public void AddRecipient(string messageType, IRecipient recipient)
        {
            if (string.IsNullOrWhiteSpace(messageType)) { throw new ArgumentException(nameof(messageType)); }
            _ = recipient ?? throw new ArgumentNullException(nameof(recipient));

            lock (this.locker)
            {
                if (this.recipients.ContainsKey(messageType))
                {
                    throw new InvalidOperationException($"Recipient for '{messageType}' has already been added.");
                }

                this.recipients[messageType] = recipient;
                this.logger?.LogInformation($"Recipient for '{messageType}' added.");
            }
        }

        /// <inheritdoc/>
        public bool HasRecipient(string messageType)
        {
            if (string.IsNullOrWhiteSpace(messageType)) { throw new ArgumentException(nameof(messageType)); }

            return this.recipients.ContainsKey(messageType);
        }

        /// <inheritdoc/>
        public void RemoveRecipient(string messageType)
        {
            if (string.IsNullOrWhiteSpace(messageType)) { throw new ArgumentException(nameof(messageType)); }

            if (!this.recipients.ContainsKey(messageType))
            {
                throw new InvalidOperationException($"No recipient for message type '{messageType}' has been added.");
            }

            this.recipients.Remove(messageType);
        }

        /// <summary>
        /// Gets the recipient.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <returns>An IRecipient.</returns>
        internal IRecipient GetRecipient(string messageType) => this.HasRecipient(messageType) ? this.recipients[messageType] : default;

        #endregion Request/Response
    }
}