/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

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
    /// <summary>
    /// The in process fire and forget channel.
    /// </summary>
    public class InProcessFireAndForgetChannel : AbstractChannel, IFireAndForgetChannel
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private readonly object locker = new object();

        /// <summary>
        /// The default period to check for "dead" messages.
        /// </summary>
        public const int DefaultPeriod = 1000;

        /// <summary>
        /// Gets the message queue.
        /// </summary>
        public IDictionary<string, ICollection<AbstractMessage>> MessageQueue { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcessFireAndForgetChannel"/> class.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <param name="logger">The logger.</param>
        internal InProcessFireAndForgetChannel(int period, ILogger logger) : base(logger)
        {
            this.MessageQueue = new Dictionary<string, ICollection<AbstractMessage>>();
            _ = new Timer(this.RemoveDeadMessages, null, period, period);
        }

        /// <summary>
        /// Fire a message with a time to live before collected and destroyed.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="timeToLive">The time to live.</param>
        public void Fire(IFireAndForgetMessage message, string timeToLive = null)
        {
            if (message.TimeToLive == default || message.TimeToLive == TimeSpan.MinValue)
            {
                if (!TimeSpan.TryParse(timeToLive, out var ttl))
                {
                    ttl = FireAndForgetMessage.DefaultTimeToLive;
                }

                message.TimeToLive = ttl;
            }

            this.AddMessage(message as AbstractMessage);
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
        /// Add message.
        /// </summary>
        /// <param name="message">The message.</param>
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

        /// <summary>
        /// Remove dead messages.
        /// </summary>
        /// <param name="state">The state.</param>
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
                        this.Logger?.LogInformation($"Message '{expired.Id}' expired ({expired.DateTime}, {((IFireAndForgetMessage)expired).TimeToLive})");
                    }
                }
            }
        }
    }
}