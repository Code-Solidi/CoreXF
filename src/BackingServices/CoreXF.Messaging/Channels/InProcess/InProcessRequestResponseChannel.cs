﻿/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
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
        /// <param name="broker">The broker.</param>
        /// <param name="logger">The logger.</param>
        internal InProcessRequestResponseChannel(IMessageBroker broker, ILogger logger) : base(logger)
        {
            this.broker = broker;
            this.logger = logger;
        }

        /// <inheritdoc/>>
        public IMessageResponse Request(IRequestMessage message)
        {
            var recipient = (this.broker as MessageBroker)?.GetRecipient(message.Type);
            return recipient == default
                ? throw new InvalidOperationException("No response, is the recipient non-null entity? Or, did the recipient supply message handler?")
                : recipient.Receive(message);
        }
    }
}