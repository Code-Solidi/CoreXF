﻿/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

namespace CoreXF.Eventing.Abstractions
{
    /// <summary>
    /// The recipient interface.
    /// </summary>
    public interface IRecipient 
    {
        /// <summary>
        /// Handle the event sent by the sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="event">The event.</param>
        void Handle(ISender sender, IEvent @event);
    }
}