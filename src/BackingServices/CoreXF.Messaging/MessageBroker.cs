using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Channels;
using CoreXF.Messaging.Abstractions.Messages;
using CoreXF.Messaging.Messages;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreXF.Messaging
{
    public class MessageBroker : IMessageBroker
    {
        private readonly IFireAndForgetChannel fireForgetChannel;
        private readonly ILogger logger;
        private readonly IPublishSubscribeChannel publishSubscribeChannel;
        private readonly IRequestResponseChannel requestResponseChannel;
        private readonly IDictionary<string, List<ISubscriber>> subscribers;
        private readonly IDictionary<string, IRecipient> recipients;
        private readonly object locker = new object();

        public MessageBroker(AbstractChannelFactory factory)
        {
            _ = factory ?? throw new ArgumentNullException(nameof(factory));

            this.subscribers = new Dictionary<string, List<ISubscriber>>();
            this.recipients = new Dictionary<string, IRecipient>();
            this.fireForgetChannel = factory.CreateFireAndForgetChannel(this);
            this.publishSubscribeChannel = factory.CreatePublishSubscribeChannel(this);
            this.requestResponseChannel = factory.CreateRequestResponseChannel(this);

            this.logger = (factory as AbstractChannelFactory).Logger;
        }

        public event FireEvent OnFire;

        public event PublishEvent OnPublish;

        public event RequestEvent OnRequest;

        #region Fire And Forget

        public bool IsRegistered(string messageType)
        {
            lock (this.locker)
            {
                return this.subscribers.ContainsKey(messageType);
            }
        }

        public void Fire(IFireAndForgetMessage message)
        {
            this.logger.LogInformation($"Firing message: {message.Id} ({message.Type}).");
            this.fireForgetChannel.Fire(message);
            this.logger.LogInformation($"Message fired: {message.Id}");
            this.OnFire?.Invoke(message);
        }

        public IEnumerable<IFireAndForgetMessage> Peek(string messageType)
        {
            var result = this.fireForgetChannel.Peek(messageType);
            return result.Cast<FireAndForgetMessage>();
        }

        public void Publish(IPublishSubscribeMessage message)
        {
            this.publishSubscribeChannel.Publish(message);
            this.OnPublish?.Invoke(message);
        }

        #endregion Fire And Forget

        #region Request/Response

        public IMessageResponse Request(IRequestResponseMessage message)
        {
            _ = message ?? throw new ArgumentNullException(nameof(message));
            this.logger.LogInformation($"Requesting: {message.Id} ({message.Type}).");
            var response = this.requestResponseChannel.Request(message);
            this.OnRequest?.Invoke(message, response);
            this.logger.LogInformation($"Response for: {message.Id} ({message.Type}) => '{response.Content}'.");
            return response;
        }

        public void AddRecipient(string messageType, IRecipient recipient)
        {
            _ = messageType ?? throw new ArgumentNullException(nameof(messageType));
            _ = recipient ?? throw new ArgumentNullException(nameof(recipient));

            lock (this.locker)
            {
                if (this.recipients.ContainsKey(messageType))
                {
                    throw new InvalidOperationException($"Recipient for '{messageType}' has already been added.");
                }

                this.recipients[messageType] = recipient;
                this.logger.LogInformation($"Recipient for '{messageType}' added.");
            }
        }

        public bool FindRecipient(string messageType)
        {
            _ = messageType ?? throw new ArgumentNullException(nameof(messageType));
            return this.recipients.ContainsKey(messageType);
        }

        public void RemoveRecipient(string messageType)
        {
            _ = messageType ?? throw new ArgumentNullException(nameof(messageType));

            if (this.recipients.ContainsKey(messageType) == false)
            {
                throw new InvalidOperationException($"No recipient for message type '{messageType}' has been added.");
            }

            this.recipients.Remove(messageType);
        }

        internal IRecipient GetRecipient(string messageType)
        {
            var recipient = default(IRecipient);
            if (this.FindRecipient(messageType))
            {
                recipient = this.recipients[messageType];
            }

            return recipient;
        }

        #endregion Request/Response

        #region Publish/Subscribe

        public void Subscribe(ISubscriber subscriber, string messageType)
        {
            _ = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
            if (string.IsNullOrWhiteSpace(messageType))
            {
                throw new ArgumentException($"Message type cannot be null or empty", nameof(subscriber));
            }

            lock (this.locker)
            {
                if (this.subscribers.ContainsKey(messageType) == false)
                {
                    throw new InvalidOperationException($"Message type '{messageType}' has not been registered. Please, register it before making a subscription.");
                }

                var messageTypeSubscribers = this.subscribers[messageType];
                if (messageTypeSubscribers.Count(x => x.Identity == subscriber.Identity) != 0)
                {
                    throw new InvalidOperationException($"A subscription for message type '{messageType}' already made.");
                }

                this.subscribers[messageType].Add(subscriber);
            }
        }

        public bool IsSubscribed(Subscriber subscriber, string messageType)
        {
            lock (this.locker)
            {
                if (this.subscribers.ContainsKey(messageType))
                {
                    var subscribers = this.subscribers[messageType];
                    return subscribers.Find(x => x.Identity == subscriber.Identity) != default;
                }

                throw new InvalidOperationException($"Message type '{messageType}' has not been registered.");
            }
        }

        public void Unsubscribe(string identity, string messageType)
        {
            lock (this.locker)
            {
                if (this.subscribers.ContainsKey(messageType))
                {
                    var subscriber = this.subscribers[messageType].SingleOrDefault(s => s.Identity == identity) ??
                        throw new InvalidOperationException($"Cannot unsubscribe, has not been subscribed to '{messageType}'.");

                    this.subscribers[messageType].Remove(subscriber);
                    return;
                }

                throw new InvalidOperationException($"Message type '{messageType}' has not been registered.");
            }
        }

        /// <summary>
        /// NB: do we need this?
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public IEnumerable<ISubscriber> GetSubscribers(string messageType)
        {
            // let it throw if key (message type) not found
            return this.subscribers[messageType];
        }

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

        #endregion Publish/Subscribe
    }
}