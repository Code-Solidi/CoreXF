/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

using CoreXF.Messaging.Abstractions;

namespace CoreXF.Messaging.Messages
{
    /// <summary>
    /// The message response.
    /// </summary>
    public class MessageResponse : IMessageResponse
    {
        /// <summary>
        /// Gets the content.
        /// </summary>
        public object Content { get; private set; }

        /// <summary>
        /// Gets the status code.
        /// </summary>
        public StatusCode StatusCode { get; private set; }

        /// <summary>
        /// Gets the recipient.
        /// </summary>
        public IRecipient Recipient { get; private set; }

        /// <summary>
        /// Gets the reason phrase.
        /// </summary>
        public string ReasonPhrase { get; private set; }

        /// <summary>
        /// Gets the default.
        /// </summary>
        public static IMessageResponse Default => new MessageResponse(default, StatusCode.Success);

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageResponse"/> class.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="reasonPhrase">The reason phrase.</param>
        public MessageResponse(IRecipient recipient, StatusCode statusCode, string reasonPhrase = default)
        {
            this.Recipient = recipient;
            this.StatusCode = statusCode;
            this.ReasonPhrase = reasonPhrase;
        }

        /// <summary>
        /// Sets the content.
        /// </summary>
        /// <param name="content">The content.</param>
        public void SetContent(object content)
        {
            this.Content = content;
        }

        /// <summary>
        /// Sets the recipient.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        public void SetRecipient(IRecipient recipient)
        {
            this.Recipient = recipient;
        }
    }
}