/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Channels;
using CoreXF.Messaging.Abstractions.Messages;
using CoreXF.Messaging.Messages;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreXF.Messaging.Channels.WebApi
{
    public class WebApiFireAndForgetChannel : WebApiChannelBase, IFireAndForgetChannel
    {
        public WebApiFireAndForgetChannel(AbstractChannelFactory factory, ILogger logger) : base(factory, logger)
        {
        }

        public async Task FireAsync(IFireAndForgetMessage message, string timeToLive)
        {
            if (message.TimeToLive == default(TimeSpan) || message.TimeToLive == TimeSpan.MinValue)
            {
                if (TimeSpan.TryParse(timeToLive, out var ttl) == false)
                {
                    ttl = FireAndForgetMessage.DefaultTimeToLive;
                }

                message.TimeToLive = ttl;
            }

            await base.SendAsync(message, this.EventBusUri);
        }

        public IEnumerable<FireAndForgetMessage> PollAll(string messageType)
        {
            var messages = base.Peek(messageType);
            return messages.Cast<FireAndForgetMessage>();
        }

        public FireAndForgetMessage Poll(Guid messageId)
        {
            if (messageId == Guid.Empty)
            {
                throw new InvalidOperationException("Polling a message with an empty GUID.");
            }

            foreach (var messageType in base.MessageTypes)
            {
                var messages = base.Peek(messageType);
                var result = messages.SingleOrDefault(x => x.Id == messageId);
                if (result != null)
                {
                    return result as FireAndForgetMessage;
                }
            }

            return null;
        }

        public void Fire(IFireAndForgetMessage message, string timeToLive = null)
        {
            throw new NotImplementedException();
        }
    }
}