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

        /// <summary>
        /// Gets the id.
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets or Sets the type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        public object Payload { get; private set; }

        /// <summary>
        /// Gets the date time.
        /// </summary>
        public DateTime DateTime { get; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>A <typeparamref name="T"></typeparamref></returns>
        public T GetPayload<T>()
        {
            try
            {
                return (T)this.Payload;
            }
            catch (InvalidCastException)
            {
                return (T)Convert.ChangeType(this.Payload, typeof(T));
            }
        }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns><![CDATA[Task<T>]]></returns>
        public async Task<T> GetPayloadAsync<T>()
        {
            return await Task.Run(() => this.GetPayload<T>()).ConfigureAwait(false);
        }
    }
}