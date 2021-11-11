/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions.Messages;

using System.Collections.Generic;

namespace CoreXF.Messaging.Abstractions.Channels
{
    public interface IChannel
    {
        IEnumerable<AbstractMessage> Peek(string messageType);

        IEnumerable<string> MessageTypes { get; }
    }
}