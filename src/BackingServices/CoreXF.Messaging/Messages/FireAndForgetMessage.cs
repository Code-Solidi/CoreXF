/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions.Messages;

using System;

namespace CoreXF.Messaging.Messages
{
    /// <summary>
    /// The fire and forget message.
    /// </summary>
    public class FireAndForgetMessage : AbstractMessage, IFireAndForgetMessage
    {
        /// <summary>
        /// Gets or Sets the time to live.
        /// </summary>
        public TimeSpan TimeToLive { get; set; }

        /// <summary>
        /// Gets or Sets the default time to live.
        /// </summary>
        public static TimeSpan DefaultTimeToLive { get; set; } = new TimeSpan(7, 0, 0, 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="FireAndForgetMessage"/> class.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        public FireAndForgetMessage(string messageType) : base(messageType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FireAndForgetMessage"/> class.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="payload">The payload.</param>
        public FireAndForgetMessage(string messageType, object payload) : base(messageType, payload)
        {
        }
    }
}