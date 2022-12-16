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
        /// The locker object.
        /// </summary>
        private readonly object locker = new object();

        /// <summary>
        /// The default period to check for "dead" messages.
        /// </summary>
        public const int DefaultPeriod = 1000;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void Fire(IFireAndForgetMessage message, TimeSpan timeToLive)
        {
            if (message.TimeToLive == default || message.TimeToLive == TimeSpan.MinValue)
            {
                message.TimeToLive = timeToLive != default ? timeToLive : FireAndForgetMessage.DefaultTimeToLive;
            }

            this.AddMessage(message as AbstractMessage);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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