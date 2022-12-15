/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

namespace CoreXF.Eventing.Abstractions
{
    /// <summary>
    /// The event interface.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Gets the payload.
        /// </summary>
        object Payload { get; }
    }
}