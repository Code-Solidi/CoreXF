/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions.Messages;

using System;
using System.Collections.Generic;

namespace CoreXF.Messaging.Abstractions.Channels
{
    /// <summary>
    /// The fire and forget channel.
    /// </summary>
    public interface IFireAndForgetChannel
    {
        /// <summary>
        /// Fires a message with a specified time to live (TTL)
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="timeToLive">The time to live.</param>
        void Fire(IFireAndForgetMessage message, string timeToLive = null);

        /// <summary>
        /// Fires a message with a specified time to live (TTL)
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="timeToLive">The time to live.</param>
        void Fire(IFireAndForgetMessage message, TimeSpan timeToLive = default);

        /// <summary>
        /// Peeks the list of abstract messages.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <returns><![CDATA[IEnumerable<AbstractMessage>]]></returns>
        IEnumerable<AbstractMessage> Peek(string messageType);
    }
}