using EventBus.Foundation.Messages;
using EventBus.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBus.Foundation
{
    public class MessageBroker : IMessageBroker
    {
        private readonly IFireForgetChannel fireForgetChannel;
        private readonly ILogger logger;
        private readonly IPublishSubscribeChannel publishSubscribeChannel;
        private readonly IRequestResponseChannel requestResponseChannel;
        private readonly IDictionary<string, List<ISubscriber>> subscribers;
        private readonly IList<IRecipient> recipients;
        private readonly object locker = new object();

        public MessageBroker(IChannelFactory factory, ILoggerFactory loggerFactory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.subscribers = new Dictionary<string, List<ISubscriber>>();
            this.recipients = new List<IRecipient>();
            this.fireForgetChannel = factory.InProcessCreateFireForgetChannel(this);
            this.publishSubscribeChannel = factory.CreatePublishSubscribeChannel(this);
            this.requestResponseChannel = factory.CreateRequestResponseChannel(this);
            this.logger = loggerFactory?.CreateLogger<MessageBroker>();// throw new ArgumentNullException(nameof(logger));
        }

        public event FireEvent OnFire;

        public event PublishEvent OnPublish;

        public event RequestEvent OnRequest;

        public IRecipient AddRecipient(IRecipient recipient)
        {
            lock (this.locker)
            {
                var found = this.recipients.SingleOrDefault(x => x.Identity == recipient.Identity);
                if (found == null)
                {
                    this.recipients.Add(recipient);
                    this.logger.LogInformation($"Recipient {recipient.Identity} added.");
                }
                else
                {
                    this.logger.LogWarning($"Recipient {recipient.Identity} has already been added.");
                }

                return recipient;
            }
        }

        public IRecipient RemoveRecipient(string identity)
        {
            lock (this.locker)
            {
                var recipient = this.recipients.SingleOrDefault(x => x.Identity == identity);
                if (recipient != null)
                {
                    this.recipients.Remove(recipient);
                    this.logger.LogInformation($"Recipient {recipient.Identity} removed.");
                }

                return recipient;
            }
        }

        public IRecipient FindRecipient(string identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            var identities = new List<string>();
            if (identity.EndsWith("MicroService", StringComparison.OrdinalIgnoreCase))
            {
                identity = identity.Remove(identity.Length - "MicroService".Length);
            }

            if (identity.EndsWith("Service", StringComparison.OrdinalIgnoreCase))
            {
                identity = identity.Remove(identity.Length - "Service".Length);
            }

            identities.Add(identity);
            identities.Add(identity + "Service");
            identities.Add(identity + "MicroService");

            return this.recipients.SingleOrDefault(x => identities.Any(t => t == x.Identity));
        }

        public void Fire(IFireForgetMessage message)
        {
            this.logger.LogInformation($"Firing message: {message.Id} ({message.Type}).");
            this.fireForgetChannel.Fire/*Async*/(message);
            this.logger.LogInformation($"Message fired: {message.Id}");
            this.OnFire?.Invoke(message);
        }

        public IEnumerable<ISubscriber> GetSubscribers(string messageType)
        {
            // let it throw if key (message type) not found
            return this.subscribers[messageType];
        }

        public IEnumerable<IMessage> Peek(string messageType)
        {
            var result = this.fireForgetChannel.Peek(messageType);
            return result.Cast<AbstractMessage>();
        }

        public void Publish(IPublishSubscribeMessage message)
        {
            this.publishSubscribeChannel.Publish(message);
            this.OnPublish?.Invoke(message);
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

        public IMessageResponse Request(IRequestResponseMessage message)
        {
            this.logger.LogInformation($"Requesting: {message.Id} ({message.Type}).");
            var response = this.requestResponseChannel.Request(message);
            this.OnRequest?.Invoke(message, response);
            this.logger.LogInformation($"Response for: {message.Id} ({message.Type}) => '{response.Content}'.");
            return response;
        }

        public ISubscriber Subscribe(ISubscriber subscriber, string messageType)
        {
            lock (this.locker)
            {
                if (this.subscribers.ContainsKey(messageType) == false)
                {
                    throw new InvalidOperationException($"Register message type '{messageType}' before creating a subscription.");
                }

                if (this.subscribers[messageType].Contains(subscriber) == true)
                {
                    throw new InvalidOperationException($"Already subscribed for message type '{messageType}'.");
                }

                this.subscribers[messageType].Add(subscriber);
                return subscriber;
            }
        }

        public ISubscriber Unsubscribe(string identity, string messageType)
        {
            lock (this.locker)
            {
                if (this.subscribers.ContainsKey(messageType) == false)
                {
                    throw new InvalidOperationException($"Message type '{messageType}' has not been registered.");
                }

                var subscriber = this.subscribers[messageType].SingleOrDefault(s => s.Identity == identity);
                if (subscriber == null)
                {
                    throw new InvalidOperationException($"Cannot unsubscribe, has not been subscribed to '{messageType}'.");
                }

                this.subscribers[messageType].Remove(subscriber);
                if (this.subscribers[messageType].Count == 0)
                {
                    this.Unregister(messageType);
                }

                return subscriber;
            }
        }
    }
}