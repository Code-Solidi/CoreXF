/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Channels;

using Microsoft.Extensions.Logging;

using System.Configuration;

namespace CoreXF.Messaging.Channels.WebApi
{
    public class WebApiChannelFactory : AbstractChannelFactory
    {
        private const int defaultPeriod = 1000;

        private readonly string eventBusUri;

        public WebApiChannelFactory(ILogger logger, int period = defaultPeriod) : base(logger)
        {
            if (period <= 0)
            {
                this.Logger.LogWarning($"Non-positive period supplied: {period}, set to default ({defaultPeriod}).");
                period = defaultPeriod;
            }

            this.eventBusUri = ConfigurationManager.AppSettings["eventBusUri"];// todo: make it constant
        }

        protected override IFireAndForgetChannel CreateFireAndForgetChannel(IMessageBroker broker)
        {
            return new WebApiFireAndForgetChannel(this, this.Logger);
        }

        protected override IPublishSubscribeChannel CreatePublishSubscribeChannel(IMessageBroker broker)
        {
            return new WebApiPublishSubscribeChannel(this, this.Logger);
        }

        protected override IRequestResponseChannel CreateRequestResponseChannel(IMessageBroker broker)
        {
            return new WebApiRequestResponseChannel(this, this.Logger);
        }
    }
}