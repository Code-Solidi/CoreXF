/*
 * Copyright (c) 2016-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

namespace CoreXF.Eventing.Abstractions
{
    public interface IMessage
    {
        object Payload { get; }
    }
}