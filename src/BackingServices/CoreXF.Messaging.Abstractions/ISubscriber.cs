﻿/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions.Messages;

namespace CoreXF.Messaging.Abstractions
{
    /// <summary>
    /// The subscriber interface.
    /// </summary>
    public interface ISubscriber
    {
        /// <summary>
        /// Gets the identity.
        /// </summary>
        string Identity { get; }

        /// <summary>
        /// Receive the message;
        /// </summary>
        /// <param name="message">The message.</param>
        void Receive(IPublishedMessage message);
    }
}