/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Eventing.Abstractions;

namespace CoreXF.Eventing
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