/*
 * Copyright (c) 2016-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the Apache License Version 2. See LICENSE.txt in the project root for license information.
 */

namespace CoreXF.Abstractions.Base
{
    /// <summary>
    /// The web api extension interface.
    /// </summary>
    public interface IWebApiExtension : IExtension
    {
        /// <summary>
        /// Gets the extension status.
        /// </summary>
        ExtensionStatus Status { get; }

        /// <summary>
        /// Starts the extension.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the extension.
        /// </summary>
        void Stop();
    }

    /// <summary>
    /// The extension status.
    /// </summary>
    public enum ExtensionStatus { Stopped, Running }
}