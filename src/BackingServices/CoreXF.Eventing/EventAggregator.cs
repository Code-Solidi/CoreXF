/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Eventing.Abstractions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CoreXF.Eventing
{
    /// <inhheritdoc/>
    public class EventAggregator : IEventAggregator
    {
        /// <inhheritdoc/>
        private readonly IDictionary<Type, IList> subscriptions = new Dictionary<Type, IList>();

        /// <inhheritdoc/>
        public void Clear(IEnumerable<Type> except)
        {
            foreach (var eventSubscriptions in new Dictionary<Type, IList>(this.subscriptions))
            {
                if (except?.Contains(eventSubscriptions.Key) == false)
                {
                    this.subscriptions.Remove(eventSubscriptions);
                }
            }
        }

        /// <inhheritdoc/>
        public void Publish(ISender sender, IEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            var eventType = @event.GetType();
            if (this.subscriptions.ContainsKey(eventType))
            {
                foreach (var subscription in this.subscriptions[eventType].Cast<Subscription>())
                {
                    subscription.Notify(sender, @event);
                }
            }
        }

        /// <inhheritdoc/>
        public void Subscribe<TEvent>(IRecipient recipient) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            var subscription = new Subscription(this, recipient, eventType);
            if (!this.subscriptions.ContainsKey(eventType))
            {
                this.subscriptions.Add(eventType, new List<Subscription>());
            }

            this.subscriptions[eventType].Add(subscription);
        }

        /// <inhheritdoc/>
        public void Unsubscribe<TEvent>(IRecipient recipient) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            if (this.subscriptions.ContainsKey(eventType))
            {
                var subscription = (IList<Subscription>)this.subscriptions[eventType];
                var entry = subscription.SingleOrDefault(x => x.Recipient == recipient);
                if (entry != default)
                {
                    subscription.Remove(entry);
                }
            }
        }

        /// <inhheritdoc/>
        private void Unsubscribe(Subscription subscription)
        {
            var eventType = this.subscriptions.SingleOrDefault(p => p.Value == subscription).Key;
            if (eventType != default && this.subscriptions.ContainsKey(eventType))
            {
                this.subscriptions[eventType].Remove(subscription);
            }
        }

        private class Subscription : IDisposable
        {
            /// <summary>
            /// Gets the event aggregator.
            /// </summary>
            public IEventAggregator EventAggregator { get; }

            /// <summary>
            /// Gets the recipient.
            /// </summary>
            public IRecipient Recipient { get; }

            /// <summary>
            /// Gets the event type.
            /// </summary>
            public Type EventType { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Subscription"/> class.
            /// </summary>
            /// <param name="eventAggregator">The event aggregator.</param>
            /// <param name="recipient">The recipient.</param>
            /// <param name="eventType">The event type.</param>
            public Subscription(IEventAggregator eventAggregator, IRecipient recipient, Type eventType)
            {
                this.EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
                this.EventType = eventType ?? throw new ArgumentNullException(nameof(eventType)); 
                this.Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
            }

            /// <summary>
            /// Notifies this.Recipient about event.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="event">The event.</param>
            public void Notify(ISender sender, IEvent @event) 
            {
                this.Recipient.Handle(sender, @event);
            }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    ((EventAggregator)this.EventAggregator).Unsubscribe(this);
                }
            }
        }
    }
}