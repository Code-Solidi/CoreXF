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
    public interface IFireAndForgetChannel //: IChannel
    {
        void Fire(IFireAndForgetMessage message, string timeToLive = null);

        IEnumerable<AbstractMessage> Peek(string messageType);
    }
}