/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using System;
using System.Threading.Tasks;

namespace CoreXF.Messaging.Abstractions.Messages
{
    public interface IMessage
    {
        Guid Id { get; }

        DateTime DateTime { get; }

        string Type { get; set; }

        T GetPayload<T>();

        Task<T> GetPayloadAsync<T>();
    }
}