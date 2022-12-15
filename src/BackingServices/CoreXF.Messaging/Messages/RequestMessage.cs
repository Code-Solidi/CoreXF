/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions.Messages;

namespace CoreXF.Messaging.Messages
{
    public class RequestMessage : AbstractMessage, IRequestMessage
    {
        public RequestMessage(string messageType, object payload) : base(messageType, payload)
        {
        }
    }
}