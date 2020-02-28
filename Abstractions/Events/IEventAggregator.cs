﻿/*
 * Copyright (c) Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

using System;
using System.Collections.Generic;

namespace CoreXF.Abstractions.Events
{
    public interface IEventAggregator
    {
        void Publish(ISender sender, IMessage message);

        void Subscribe<TMessage>(IRecipient recipient) where TMessage : IMessage;

        void UnSubscribe<TMessage>(IRecipient recipient) where TMessage : IMessage;

        void ClearAllSubscriptions(IEnumerable<Type> exceptMessages = null);
    }
}