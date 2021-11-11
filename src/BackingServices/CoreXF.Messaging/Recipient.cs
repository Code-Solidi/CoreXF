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
    public class Recipient : IRecipient
    {
        public string Identity { get; }

        public Recipient(string identity)
        {
            this.Identity = identity;
        }

        public virtual IMessageResponse Recieve(IRequestMessage message)
        {
            _ = message ?? throw new ArgumentNullException(nameof(message));
            return MessageResponse.Default;
        }
    }
}