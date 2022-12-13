/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions.Messages;

namespace CoreXF.Messaging.Abstractions.Channels
{
    /// <summary>
    /// The publish subscribe channel interface.
    /// </summary>
    public interface IPublishSubscribeChannel
    {
        /// <summary>
        /// Publish the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Publish(IPublishedMessage message);
    }
}