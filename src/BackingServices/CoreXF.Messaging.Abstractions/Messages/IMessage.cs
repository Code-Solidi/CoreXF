/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using System;
using System.Threading.Tasks;

namespace CoreXF.Messaging.Abstractions.Messages
{
    /// <summary>
    /// The message interface.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the date time.
        /// </summary>
        DateTime DateTime { get; }

        /// <summary>
        /// Gets or Sets the type.
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>A <typeparamref name="T"></typeparamref></returns>
        T GetPayload<T>();

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An object.</returns>
        object GetPayload(Type type);

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns><![CDATA[Task<T>]]></returns>
        Task<T> GetPayloadAsync<T>();

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><![CDATA[Task<object>]]></returns>
        Task<object> GetPayloadAsync(Type type);
    }
}