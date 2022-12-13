/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Channels;

using Microsoft.Extensions.Logging;

using System;

namespace CoreXF.Messaging.Channels.InProcess
{
    /// <summary>
    /// The in process channel factory.
    /// </summary>
    public class InProcessChannelFactory : AbstractChannelFactory
    {
        /// <summary>
        /// The default period.
        /// </summary>
        public const int DefaultPeriod = 1000;

        /// <summary>
        /// The period.
        /// </summary>
        private readonly int period;

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcessChannelFactory"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="period">The period.</param>
        public InProcessChannelFactory(ILoggerFactory loggerFactory, int period = InProcessChannelFactory.DefaultPeriod)
            : base(loggerFactory?.CreateLogger<InProcessChannelFactory>())
        {
            this.period = period > 0
                ? period
                : throw new ArgumentException($"Non-positive period supplied: {period}, set to default ({InProcessChannelFactory.DefaultPeriod}).", nameof(period));
        }

        /// <summary>
        /// Create the fire and forget channel.
        /// </summary>
        /// <param name="broker">The broker.</param>
        /// <returns>An IFireAndForgetChannel.</returns>
        public override IFireAndForgetChannel CreateFireAndForgetChannel(IMessageBroker broker) => new InProcessFireAndForgetChannel(this, this.period, this.Logger);

        /// <summary>
        /// Creates the publish subscribe channel.
        /// </summary>
        /// <param name="broker">The broker.</param>
        /// <returns>An IPublishSubscribeChannel.</returns>
        public override IPublishSubscribeChannel CreatePublishSubscribeChannel(IMessageBroker broker) => new InProcessPublishSubscriberChannel(this, broker, this.Logger);

        /// <summary>
        /// Create the request response channel.
        /// </summary>
        /// <param name="broker">The broker.</param>
        /// <returns>An IRequestResponseChannel.</returns>
        public override IRequestResponseChannel CreateRequestResponseChannel(IMessageBroker broker) => new InProcessRequestResponseChannel(this, broker, this.Logger);
    }
}