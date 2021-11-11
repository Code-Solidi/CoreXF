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
    public class EventAggregator : IEventAggregator
    {
        private readonly IDictionary<Type, IList> subscriptions = new Dictionary<Type, IList>();

        public void ClearAllSubscriptions(IEnumerable<Type> exceptMessages)
        {
            foreach (var messageSubscriptions in new Dictionary<Type, IList>(this.subscriptions))
            {
                if (exceptMessages?.Contains(messageSubscriptions.Key) == false)
                {
                    this.subscriptions.Remove(messageSubscriptions);
                }
            }
        }

        public void Publish(ISender sender, IMessage message)
        {
            _ = message ?? throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();
            if (this.subscriptions.ContainsKey(messageType))
            {
                foreach (var subscription in this.subscriptions[messageType].Cast<Subscription>())
                {
                    var recipient = subscription.Recipient;
                    recipient.Handle(sender, message);
                }
            }
        }

        public void Subscribe<TMessage>(IRecipient recipient) where TMessage : IMessage
        {
            var messageType = typeof(TMessage);
            var subscription = new Subscription(this, recipient, messageType);
            if (this.subscriptions.ContainsKey(messageType) == false)
            {
                this.subscriptions.Add(messageType, new List<Subscription>());
            }

            this.subscriptions[messageType].Add(subscription);
        }

        public void UnSubscribe<TMessage>(IRecipient recipient) where TMessage : IMessage
        {
            var messageType = typeof(TMessage);
            if (this.subscriptions.ContainsKey(messageType))
            {
                var subscription = (IList<Subscription>)this.subscriptions[messageType];
                var entry = subscription.SingleOrDefault(x => x.Recipient == recipient);
                if (entry != default)
                {
                    subscription.Remove(entry);
                }
            }
        }

        private void UnSubscribe(Subscription subscription)
        {
            var messageType = this.subscriptions.SingleOrDefault(p => p.Value == subscription).Key;
            if (messageType != default)
            {
                if (this.subscriptions.ContainsKey(messageType))
                {
                    this.subscriptions[messageType].Remove(subscription);
                }
            }
        }

        private class Subscription : IDisposable
        {
            public IEventAggregator EventAggregator { get; }

            public IRecipient Recipient { get; }

            public Type MessageType { get; }

            public Subscription(IEventAggregator eventAggregator, IRecipient recipient, Type messageType)
            {
                this.EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
                this.MessageType = messageType ?? throw new ArgumentNullException(nameof(messageType)); ;
                this.Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
            }

            public void Notify<TSender>(TSender sender, IMessage message)
            {
                throw new NotImplementedException();
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
                    ((EventAggregator)this.EventAggregator).UnSubscribe(this);
                }
            }
        }
    }
}