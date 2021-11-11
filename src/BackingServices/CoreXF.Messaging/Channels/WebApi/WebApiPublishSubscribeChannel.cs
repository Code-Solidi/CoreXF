/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Channels;
using CoreXF.Messaging.Abstractions.Messages;

using Microsoft.Extensions.Logging;

namespace CoreXF.Messaging.Channels.WebApi
{
    public class WebApiPublishSubscribeChannel : WebApiChannelBase, IPublishSubscribeChannel
    {
        public WebApiPublishSubscribeChannel(AbstractChannelFactory factory, ILogger logger) : base(factory, logger)
        {
        }

        public void Publish(IPublishSubscribeMessage message)
        {
            base.SendAsync(message, this.EventBusUri).GetAwaiter().GetResult();
        }
    }
}