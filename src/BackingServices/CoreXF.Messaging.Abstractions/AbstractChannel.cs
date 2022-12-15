/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using Microsoft.Extensions.Logging;

namespace CoreXF.Messaging.Abstractions
{
    /// <summary>
    /// The abstract channel.
    /// </summary>
    public abstract class AbstractChannel 
    {
        /// <summary>
        /// The factory.
        /// </summary>
        //private readonly AbstractChannelFactory factory;

        /// <summary>
        /// Gets or Sets the logger.
        /// </summary>
        protected internal ILogger Logger { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractChannel"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="logger">The logger.</param>
        protected AbstractChannel(/*AbstractChannelFactory factory, */ILogger logger)
        {
            this.Logger = logger;
            //this.factory = factory;
        }
    }
}