/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions.Messages;

namespace CoreXF.Messaging.Abstractions
{
    /// <summary>
    /// The recipient interface. Implementors participate in Request/Response channels.
    /// </summary>
    public interface IRecipient
    {
        /// <summary>
        /// Gets the identity.
        /// </summary>
        string Identity { get; }

        /// <summary>
        /// Receive the <see cref="IMessageResponse"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>An IMessageResponse.</returns>
        IMessageResponse Receive(IRequestMessage message);
    }
}