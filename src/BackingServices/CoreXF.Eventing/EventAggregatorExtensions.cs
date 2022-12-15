/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Eventing.Abstractions;

namespace CoreXF.Eventing
{
    /// <summary>
    /// The event aggregator extensions.
    /// </summary>
    public static class EventAggregatorExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="recipient">The recipient.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public static void Subscribe<TEvent>(this IRecipient recipient, IEventAggregator eventAggregator) where TEvent : IEvent
        {
            eventAggregator.Subscribe<TEvent>(recipient);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="recipient">The recipient.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public static void Unsubscribe<TEvent>(this IRecipient recipient, IEventAggregator eventAggregator) where TEvent : IEvent
        {
            eventAggregator.Unsubscribe<TEvent>(recipient);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="sender">The sender.</param>
        /// <param name="event">The event.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public static void Publish<TEvent>(this ISender sender, TEvent @event, IEventAggregator eventAggregator) where TEvent : IEvent
        {
            eventAggregator.Publish(sender, @event);
        }
    }
}