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
    public class InProcessRequestResponseChannel : AbstractChannel, IRequestResponseChannel
    {
        private readonly IMessageBroker broker;

        private readonly ILogger logger;

        internal InProcessRequestResponseChannel(AbstractChannelFactory factory, IMessageBroker broker, ILogger logger)
            : base(factory, logger)
        {
            this.broker = broker;
            this.logger = logger;
        }

        public IMessageResponse Request(IRequestMessage message)
        {
            var recipient = (this.broker as MessageBroker).GetRecipient(message.Type);
            if (recipient == default)
            {
                throw new InvalidOperationException("No response, is the recipient non-null entity? Or, did the recipient supply message handler?");
            }

            return recipient.Recieve(message);
        }
    }
}