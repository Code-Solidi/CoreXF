/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions.Messages;

using System;

namespace CoreXF.Messaging.Messages
{
    public class FireAndForgetMessage : AbstractMessage, IFireAndForgetMessage
    {
        public TimeSpan TimeToLive { get; set; }

        public static TimeSpan DefaultTimeToLive { get; set; } = new TimeSpan(7, 0, 0, 0);

        public FireAndForgetMessage(string messageType) : base(messageType)
        {
        }

        public FireAndForgetMessage(string messageType, object payload) : base(messageType, payload)
        {
        }
    }
}