/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

namespace CoreXF.Eventing.Abstractions
{
    public interface ISender //: IExtension
    {
        //string Name { get; }

        void Publish<TMessage>(TMessage message, IEventAggregator eventAggregator) where TMessage : IEvent;
    }
}