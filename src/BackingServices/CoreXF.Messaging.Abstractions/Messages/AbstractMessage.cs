/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using System;
using System.Threading.Tasks;

namespace CoreXF.Messaging.Abstractions.Messages
{
    public abstract class AbstractMessage : IMessage
    {
        protected AbstractMessage(string messageType)
        {
            this.Type = string.IsNullOrWhiteSpace(messageType) ? throw new ArgumentException("Message type cannot be null or empty") : messageType;
        }

        protected AbstractMessage(string messageType, object payload) : this(messageType)
        {
            this.Payload = payload;
        }

        public Guid Id { get; } = Guid.NewGuid();

        public string Type { get; set; }

        public object Payload { get; private set; }

        public DateTime DateTime { get; } = DateTime.UtcNow;

        public T GetPayload<T>()
        {
            var result = default(T);
            try
            {
                result = (T)this.Payload;
            }
            catch (InvalidCastException)
            {
                result = (T)Convert.ChangeType(this.Payload, typeof(T));
            }

            return result;
        }

        public async Task<T> GetPayloadAsync<T>()
        {
            return await Task.Run(() => this.GetPayload<T>()).ConfigureAwait(false);
        }
    }
}