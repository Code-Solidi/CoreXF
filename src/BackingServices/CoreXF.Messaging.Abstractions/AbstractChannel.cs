/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using Microsoft.Extensions.Logging;

namespace CoreXF.Messaging.Abstractions
{
    public abstract class AbstractChannel //: IChannel
    {
        private readonly AbstractChannelFactory factory;

        protected internal ILogger Logger { get; set; }

        protected AbstractChannel(AbstractChannelFactory factory, ILogger logger)
        {
            this.Logger = logger;
            this.factory = factory;
        }
    }
}