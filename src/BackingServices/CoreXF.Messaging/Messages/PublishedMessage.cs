/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions.Messages;

namespace CoreXF.Messaging.Messages
{
    /// <summary>
    /// The published message.
    /// </summary>
    public class PublishedMessage : AbstractMessage, IPublishedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublishedMessage"/> class.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="payload">The payload.</param>
        public PublishedMessage(string messageType, object payload) : base(messageType, payload)
        {
        }
    }
}