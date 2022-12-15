/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using System;
using System.Threading.Tasks;

namespace CoreXF.Messaging.Abstractions.Messages
{
    /// <summary>
    /// The abstract message.
    /// </summary>
    public abstract class AbstractMessage : IMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractMessage"/> class.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        protected AbstractMessage(string messageType)
        {
            this.Type = string.IsNullOrWhiteSpace(messageType) ? throw new ArgumentException("Message type cannot be null or empty") : messageType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractMessage"/> class.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="payload">The payload.</param>
        protected AbstractMessage(string messageType, object payload) : this(messageType)
        {
            this.Payload = payload;
        }

        /// <inheritdoc/>
        public Guid Id { get; } = Guid.NewGuid();

        /// <inheritdoc/>
        public string Type { get; set; }

        /// <inheritdoc/>
        public object Payload { get; }

        /// <inheritdoc/>
        public DateTime DateTime { get; } = DateTime.UtcNow;

        /// <inheritdoc/>
        public T GetPayload<T>()
        {
            try
            {
                return (T)this.Payload;
            }
            catch (InvalidCastException)
            {
                return default;
            }
        }

        /// <inheritdoc/>
        public object GetPayload(Type type)
        {
            try
            {
                return Convert.ChangeType(this.Payload, type);
            }
            catch (InvalidCastException)
            {
                return default;
            }
        }

        /// <inheritdoc/>
        public async Task<T> GetPayloadAsync<T>()
        {
            return await Task.Run(() => this.GetPayload<T>()).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<object> GetPayloadAsync(Type type)
        {
            return await Task.Run(() => this.GetPayload(type)).ConfigureAwait(false);
        }
    }
}