/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions.Messages;

using System.Collections.Generic;

namespace CoreXF.Messaging.Abstractions
{
    /// <summary>
    /// The message broker interface.
    /// </summary>
    public interface IMessageBroker
    {
        #region Fire And Forget (Broadcast)

        /// <summary>Fires the specified message.</summary>
        /// <param name="message">The message.</param>
        void Fire(IFireAndForgetMessage message);

        /// <summary>
        /// Peeks the specified message type.
        /// </summary>
        /// <param name="messageType">
        /// Type of the message.
        /// </param>
        /// <returns></returns>
        IEnumerable<IFireAndForgetMessage> Peek(string messageType);

        #endregion Fire And Forget (Broadcast)

        #region Publish/Subscribe (Multicast)

        /// <summary>Registers the specified message type.</summary>
        /// <param name="messageType">Type of the message.</param>
        void Register(string messageType);

        /// <summary>Unregisters the specified message type.</summary>
        /// <param name="messageType">Type of the message.</param>
        void Unregister(string messageType);

        /// <summary>Determines whether the specified message type is registered.</summary>
        /// <param name="messageType">Type of the message.</param>
        /// <returns>
        ///   <c>true</c> if the specified message type is registered; otherwise, <c>false</c>.</returns>
        bool IsRegistered(string messageType);

        /// <summary>Publishes the specified message.</summary>
        /// <param name="message">The message.</param>
        void Publish(IPublishedMessage message);

        /// <summary>Subscribes the specified subscriber.</summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="messageType">Type of the message.</param>
        void Subscribe(ISubscriber subscriber, string messageType);

        /// <summary>Unsubscribes the specified identity.</summary>
        /// <param name="identity">The identity.</param>
        /// <param name="messageType">Type of the message.</param>
        void Unsubscribe(string identity, string messageType);

        /// <summary>Determines whether the specified subscriber is subscribed.</summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <returns>
        ///   <c>true</c> if the specified subscriber is subscribed; otherwise, <c>false</c>.</returns>
        bool IsSubscribed(ISubscriber subscriber, string messageType);

        #endregion Publish/Subscribe (Multicast)

        #region Request/Response (Unicast)

        /// <summary>Adds the recipient.</summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="recipient">The recipient.</param>
        /// <remarks>The message type is automatically registered.</remarks>
        void AddRecipient(string messageType, IRecipient recipient);

        /// <summary>Determines if messageType has a recipient.</summary>
        /// <param name="messageType">Type of the message.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        bool HasRecipient(string messageType);

        /// <summary>Removes the recipient.</summary>
        /// <param name="messageType">Type of the message.</param>
        /// <remarks>The message type is automatically unregistered.</remarks>
        void RemoveRecipient(string messageType);

        /// <summary>Requests the specified message.</summary>
        /// <param name="message">The message.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <remarks>If the message type is not registered fires an exception.</remarks>
        IMessageResponse Request(IRequestMessage message);

        #endregion Request/Response (Unicast)

        event FireEvent OnFire;

        event ResponseEvent OnResponse;
    }
}