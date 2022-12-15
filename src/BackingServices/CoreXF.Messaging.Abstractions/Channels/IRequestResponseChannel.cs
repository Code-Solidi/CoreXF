/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions.Messages;

namespace CoreXF.Messaging.Abstractions.Channels
{
    /// <summary>
    /// The request response channel interface.
    /// </summary>
    public interface IRequestResponseChannel //: IChannel
    {
        /// <summary>
        /// Sends a <see cref="IRequestMessage"/> to the registered recipient.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>An <see cref="IMessageResponse"/>.</returns>
        IMessageResponse Request(IRequestMessage message);
    }
}