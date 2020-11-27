/*
 * Copyright (c) 2017-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using CoreXF.Abstractions.Events;

namespace CoreXF.Framework.Events
{
    public static class EventAggregatorExtensions
    {
        public static void Subscribe<TMessage>(this IRecipient recipient, IEventAggregator eventAggregator) where TMessage : IMessage
        {
            eventAggregator.Subscribe<TMessage>(recipient);
        }

        public static void Unsubscribe<TMessage>(this IRecipient recipient, IEventAggregator eventAggregator) where TMessage : IMessage
        {
            eventAggregator.UnSubscribe<TMessage>(recipient);
        }

        public static void Publish<TMessage>(this ISender sender, TMessage message, IEventAggregator eventAggregator) where TMessage : IMessage
        {
            eventAggregator.Publish(sender, message);
        }
    }
}