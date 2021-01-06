using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Channels;
using CoreXF.Messaging.Abstractions.Messages;
using CoreXF.Messaging.Messages;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CoreXF.Messaging.Channels.InProcess
{
    public class InProcessFireAndForgetChannel : AbstractChannel, IFireAndForgetChannel
    {
        private readonly object locker = new object();
        private readonly InProcessChannelFactory factory;
        public const int DefaultPeriod = 1000;
        private readonly Timer timer;

        public IDictionary<string, ICollection<AbstractMessage>> MessageQueue { get; private set; }

        internal InProcessFireAndForgetChannel(InProcessChannelFactory factory, int period, ILogger logger)
            : base(factory, logger)
        {
            this.factory = factory;
            this.MessageQueue = new Dictionary<string, ICollection<AbstractMessage>>();
            this.timer = new Timer(this.RemoveDeadMessages, null, period, period);
        }

        public void Fire(IFireAndForgetMessage message, string timeToLive = null)
        {
            if (message.TimeToLive == default || message.TimeToLive == TimeSpan.MinValue)
            {
                if (TimeSpan.TryParse(timeToLive, out var ttl) == false)
                {
                    ttl = FireAndForgetMessage.DefaultTimeToLive;
                }

                message.TimeToLive = ttl;
            }

            this.AddMessage(message as AbstractMessage);
        }

        public IEnumerable<IMessage> GetAllMessages()
        {
            var list = new List<IMessage>();
            foreach (var item in this.MessageQueue.Values)
            {
                list.AddRange(item);
            }

            return list;
        }

        /// <summary>
        /// Peeks the specified message type.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <returns></returns>
        public virtual IEnumerable<AbstractMessage> Peek(string messageType)
        {
            lock (this.locker)
            {
                return this.MessageQueue[messageType];
            }
        }

        /// <summary>
        /// Gets the message types.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> MessageTypes => this.MessageQueue.Keys;

        internal void AddMessage(AbstractMessage message)
        {
            lock (this.locker)
            {
                // NB: this is registration in fact, done silently
                if (this.MessageQueue.ContainsKey(message.Type) == false)
                {
                    this.MessageQueue[message.Type] = new List<AbstractMessage>();
                }

                this.MessageQueue[message.Type].Add(message);
            }
        }

        protected virtual void RemoveDeadMessages(object state)
        {
            var lists = new Dictionary<string, List<AbstractMessage>>();
            lock (this.locker)
            {
                foreach (var item in this.MessageQueue)
                {
                    var expired = item.Value.Where(m => DateTime.UtcNow - m.DateTime > ((IFireAndForgetMessage)m).TimeToLive);
                    if (expired.Any())
                    {
                        lists[item.Key] = new List<AbstractMessage>(expired.Cast<AbstractMessage>());
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