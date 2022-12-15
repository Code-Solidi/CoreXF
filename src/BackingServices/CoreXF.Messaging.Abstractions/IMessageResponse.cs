/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

namespace CoreXF.Messaging.Abstractions
{
    /// <summary>
    /// The message response.
    /// </summary>
    public interface IMessageResponse
    {
        object Content { get; }

        StatusCode StatusCode { get; }

        IRecipient Recipient { get; }

        string ReasonPhrase { get; }
    }

    public enum StatusCode
    {
        Success,

        Failed
    }
}