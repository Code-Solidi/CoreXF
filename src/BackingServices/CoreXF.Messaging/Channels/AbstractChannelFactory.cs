using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Channels;
using CoreXF.Messaging.Abstractions.Messages;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreXF.Messaging.Channels
{
    public abstract class AbstractChannelFactory 
    {
        private readonly ILogger logger;

        private readonly object locker = new object();

        public IDictionary<string, ICollection<IMessage>> MessageQueue { get; private set; }

        internal ILogger Logger => this.logger;

        public IEnumerable<IMessage> GetAllMessages()
        {
            var list = new List<IMessage>();
            foreach (var item in this.MessageQueue.Values)
            {
                list.AddRange(item);
            }

            return list;
        }

        protected AbstractChannelFactory(ILogger logger)
        {
            this.logger = logger;
            this.MessageQueue = new Dictionary<string, ICollection<IMessage>>();
        }

        internal abstract IFireAndForgetChannel CreateFireAndForgetChannel(IMessageBroker broker);

        internal abstract IPublishSubscribeChannel CreatePublishSubscribeChannel(IMessageBroker broker);

        internal abstract IRequestResponseChannel CreateRequestResponseChannel(IMessageBroker broker);

        protected internal virtual void AddMessage(IMessage message)
        {
            lock (this.locker)
            {
                // NB: this is registration in fact, done silently
                if (this.MessageQueue.ContainsKey(message.Type) == false)
                {
                    this.MessageQueue[message.Type] = new List<IMessage>();
                }

                this.MessageQueue[message.Type].Add(message);
            }
        }

        protected virtual void RemoveDeadMessages(object state)
        {
            var lists = new Dictionary<string, List<IMessage>>();
            lock (this.locker)
            {
                foreach (var item in this.MessageQueue)
                {
                    var expired = item.Value.OfType<IFireAndForgetMessage>().Where(m => DateTime.UtcNow - m.DateTime > m.TimeToLive);
                    if (expired.Any())
                    {
                        lists[item.Key] = new List<IMessage>(expired);
                    }
                }

                foreach (var list in lists)
                {
                    var messageList = this.MessageQueue[list.Key];
                    foreach (var expired in list.Value)
                    {
                        messageList.Remove(expired);
                        this.Logger.LogInformation($"Message '{expired.Id}' expired ({expired.DateTime}, {((IFireAndForgetMessage)expired).TimeToLive})");
                    }
                }
            }
        }
    }
}