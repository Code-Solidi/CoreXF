/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions;
using CoreXF.Messaging.Abstractions.Messages;

namespace CoreXF.Messaging
{
    /// <summary>
    /// This class is registered with the event bus' publisher controller. Each entity which needs to be notified about some event registers a
    /// subscriber with the messageType (usually typeof(&lt;MessageDescendedn&gt;).FullName) with the event bus. Once the entity is finished with
    /// these event (Message) types it should unregister itself.
    /// </summary>
    public abstract class Subscriber : ISubscriber
    {
        /// <summary>
        /// Gets the identity.
        /// </summary>
        public string Identity { get; }

        //public event RecieveEvent OnRecieve;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscriber"/> class.
        /// </summary>
        /// <param name="identity">The identity.</param>
        protected Subscriber(string identity)
        {
            this.Identity = identity;
        }

        /// <summary>
        /// Receive the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public abstract void Receive(IPublishedMessage message);
    }
}