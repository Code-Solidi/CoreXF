/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using System;
using System.Collections.Generic;

namespace CoreXF.Eventing.Abstractions
{
    /// <summary>
    /// The event aggregator.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// Publishes an event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="event">The event.</param>
        void Publish(ISender sender, IEvent @event);

        /// <summary>
        /// Subscribes recipient to an event.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="recipient">The recipient.</param>
        void Subscribe<TEvent>(IRecipient recipient) where TEvent : IEvent;

        /// <summary>
        /// Unsubscribes recipient from an event.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="recipient">The recipient.</param>
        void Unsubscribe<TEvent>(IRecipient recipient) where TEvent : IEvent;

        /// <summary>
        /// Clears all event types except 'except'.
        /// </summary>
        /// <param name="except">The except.</param>
        void Clear(IEnumerable<Type> except = null);
    }
}