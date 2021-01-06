using EventBus.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBus.Foundation.Channels
{
    public abstract class AbstractFactory : IChannelFactory
    {
        protected readonly ILogger logger;

        private readonly object locker = new object();

        public IDictionary<string, ICollection<IMessage>> MessageQueue { get; private set; }

        public IEnumerable<IMessage> GetAllMessages()
        {
            var list = new List<IMessage>();
            foreach (var item in this.MessageQueue.Values)
            {
                list.AddRange(item);
            }

            return list;
        }

        protected AbstractFactory(ILogger logger)
        {
            this.logger = logger;
            this.MessageQueue = new Dictionary<string, ICollection<IMessage>>();
        }

        //protected virtual T CreateChannel<T>() where T : IChannel
        //{
        //    var channel = (T)Activator.CreateInstance(typeof(T), true);
        //    (channel as AbstractChannel).Logger = this.logger;
        //    return channel;
        //}

        public abstract IFireForgetChannel InProcessCreateFireForgetChannel(IMessageBroker broker);

        public abstract IPublishSubscribeChannel CreatePublishSubscribeChannel(IMessageBroker broker);

        public abstract IRequestResponseChannel CreateRequestResponseChannel(IMessageBroker broker);

        protected internal virtual void AddMessage(IMessage message)
        {
            lock (this.locker)
            {
                // nb: this is registration in fact, done silently
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
                    var expired = item.Value.OfType<IFireForgetMessage>().Where(m => DateTime.UtcNow - m.DateTime > m.TimeToLive);
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
                        this.logger.LogInformation($"Message '{expired.Id}' expired ({expired.DateTime}, {((IFireForgetMessage)expired).TimeToLive})");
                    }
                }
            }
        }
    }
}