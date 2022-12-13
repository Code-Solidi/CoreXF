/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Abstractions.Base;

namespace CoreXF.Eventing.Abstractions
{
    public interface IRecipient //: IExtension
    {
        //string Name { get; }

        void Handle(ISender sender, IEvent @event);
    }
}