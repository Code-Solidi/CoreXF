/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

namespace CoreXF.Messaging.Abstractions.Messages
{
    public delegate void RecieveEvent(IPublishedMessage message, out IMessageResponse response);

    public delegate void PublishEvent(IPublishedMessage message);

    public delegate void FireEvent(IFireAndForgetMessage message);

    public delegate void ForgetEvent(IFireAndForgetMessage message);

    public delegate void ResponseEvent(IRequestMessage message, IMessageResponse response);
}