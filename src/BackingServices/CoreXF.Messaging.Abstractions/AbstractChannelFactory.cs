/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions.Channels;

using Microsoft.Extensions.Logging;

namespace CoreXF.Messaging.Abstractions
{
    /// <summary>
    /// The abstract channel factory.
    /// </summary>
    public abstract class AbstractChannelFactory
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger => this.logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractChannelFactory"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected AbstractChannelFactory(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Create the fire and forget channel.
        /// </summary>
        /// <param name="broker">The broker.</param>
        /// <returns>An IFireAndForgetChannel.</returns>
        public abstract IFireAndForgetChannel CreateFireAndForgetChannel(IMessageBroker broker);

        /// <summary>
        /// Creates the publish subscribe channel.
        /// </summary>
        /// <param name="broker">The broker.</param>
        /// <returns>An IPublishSubscribeChannel.</returns>
        public abstract IPublishSubscribeChannel CreatePublishSubscribeChannel(IMessageBroker broker);

        /// <summary>
        /// Create the request response channel.
        /// </summary>
        /// <param name="broker">The broker.</param>
        /// <returns>An IRequestResponseChannel.</returns>
        public abstract IRequestResponseChannel CreateRequestResponseChannel(IMessageBroker broker);
    }
}