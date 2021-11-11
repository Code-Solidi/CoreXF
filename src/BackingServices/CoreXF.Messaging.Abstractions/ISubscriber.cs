/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions.Messages;

namespace CoreXF.Messaging.Abstractions
{
    public interface ISubscriber
    {
        string Identity { get; }

        void Recieve(IPublishedMessage message);
    }
}