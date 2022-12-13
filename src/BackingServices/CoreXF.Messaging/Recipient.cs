/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Messages;
using CoreXF.Messaging.Messages;

using System;

namespace CoreXF.Messaging
{
    /// <summary>
    /// The recipient.
    /// </summary>
    public abstract class Recipient : IRecipient
    {
        /// <summary>
        /// Gets the identity.
        /// </summary>
        public string Identity { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Recipient"/> class.
        /// </summary>
        /// <param name="identity">The identity.</param>
        protected Recipient(string identity)
        {
            this.Identity = identity;
        }

        /// <summary>
        /// Receives the <see cref="IMessageResponse"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An IMessageResponse.</returns>
        public abstract IMessageResponse Receive(IRequestMessage message);
    }
}