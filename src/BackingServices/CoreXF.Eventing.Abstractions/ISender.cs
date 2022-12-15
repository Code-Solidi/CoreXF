/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

namespace CoreXF.Eventing.Abstractions
{
    /// <summary>
    /// The sender interface.
    /// </summary>
    public interface ISender //: IExtension
    {
        //string Name { get; }

        /// <summary>
        /// TODO: Add Summary
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="event">The event.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        void Publish<TEvent>(TEvent @event, IEventAggregator eventAggregator) where TEvent : IEvent;
    }
}