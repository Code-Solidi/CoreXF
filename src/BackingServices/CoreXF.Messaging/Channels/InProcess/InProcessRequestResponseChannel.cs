/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Channels;
using CoreXF.Messaging.Abstractions.Messages;

using Microsoft.Extensions.Logging;

using System;

namespace CoreXF.Messaging.Channels.InProcess
{
    /// <summary>
    /// The in process request response channel.
    /// </summary>
    public class InProcessRequestResponseChannel : AbstractChannel, IRequestResponseChannel
    {
        private readonly IMessageBroker broker;

        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcessRequestResponseChannel"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="broker">The broker.</param>
        /// <param name="logger">The logger.</param>
        internal InProcessRequestResponseChannel(AbstractChannelFactory factory, IMessageBroker broker, ILogger logger)
            : base(factory, logger)
        {
            this.broker = broker;
            this.logger = logger;
        }

        /// <summary>
        /// Request the <see cref="IMessageResponse"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>An IMessageResponse.</returns>
        public IMessageResponse Request(IRequestMessage message)
        {
            var recipient = (this.broker as MessageBroker)?.GetRecipient(message.Type);
            return recipient == default
                ? throw new InvalidOperationException("No response, is the recipient non-null entity? Or, did the recipient supply message handler?")
                : recipient.Receive(message);
        }
    }
}