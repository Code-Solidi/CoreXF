/*
 * Copyright (c) 2017-2020 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

namespace Eventing.Abstractions
{
    public interface IRecipient
    {
        string Name { get; }

        void Handle(ISender sender, IMessage message);
    }
}