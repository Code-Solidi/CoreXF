/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions.Messages;

using System.Collections.Generic;

namespace CoreXF.Messaging.Abstractions.Channels
{
    /// <summary>
    /// The fire and forget channel.
    /// </summary>
    public interface IFireAndForgetChannel
    {
        /// <summary>
        /// TODO: Add Summary
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="timeToLive">The time to live.</param>
        void Fire(IFireAndForgetMessage message, string timeToLive = null);

        /// <summary>
        /// Peeks the list of abstract messages.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <returns><![CDATA[IEnumerable<AbstractMessage>]]></returns>
        IEnumerable<AbstractMessage> Peek(string messageType);
    }
}