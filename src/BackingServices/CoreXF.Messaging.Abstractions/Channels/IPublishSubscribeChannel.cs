﻿/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions.Messages;

namespace CoreXF.Messaging.Abstractions.Channels
{
    public interface IPublishSubscribeChannel //: IChannel
    {
        void Publish(IPublishedMessage message);
    }
}